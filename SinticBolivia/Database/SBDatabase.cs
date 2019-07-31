using System;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.Data.Common;

namespace SinticBolivia.Database
{
	
	public abstract class SBDatabase
	{
		public string db_type;
        protected string connectionString;
		protected long lastInserId;
        protected bool _transactionOpen = false;
        public string LastQuery;
		public long LastInsertID{get{return this.lastInserId;}}
		public abstract bool open();
		public abstract bool close();
        public abstract void BeginTransaction();
        public abstract void EndTransaction();
        /// <summary>
        /// Prepares the columns based on database engine.
        /// </summary>
        /// <returns>The columns.</returns>
        /// <param name="columns">Columns.</param>
        public abstract string PrepareColumns(string[] columns);
		public abstract ArrayList Query(string query = "");
        public abstract ArrayList ExecuteProcedure(string procedureName, Hashtable procParameters);
		public abstract int Execute(string query);
		public abstract string escapeString(string str);
        public abstract bool CreateTable(string table_name, object cols, string[] primary_key = null, bool auto_increment = true);
		protected string datetime_format = CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern;
        protected string _builtQuery = "";

		public object getTableCode(string table, string primary_key)
        {
            string query = "SELECT MAX(" + primary_key + ") AS next FROM " + table + " ";
            Hashtable res = this.QueryRow(query);
            if ( res == null || res["next"] == null)
                return (object)"1";
			if( String.IsNullOrEmpty(res["next"].ToString()) )
				return (object)"1";
            int next = Int32.Parse(res["next"].ToString()) + 1;
            return (object)next;
        }
        public Hashtable QueryRow(string query = "")
        {
            if (string.IsNullOrEmpty(query))
                query = this._builtQuery;

            Hashtable row = new Hashtable();
            ArrayList rows = this.Query(query);
            this._builtQuery = "";
            if (rows == null)
                return null;
			if( rows.Count > 0)
            	row = (Hashtable)rows[0];
            else
				row = null;
            return row;
        }
        public int insert(string table, Hashtable data)
        {
			return this.Insert(table, data);
        }
        public int insert(string table, object data)
        {
			return this.Insert(table, data);
        }
		public string FormatValue(object the_value)
		{
			string sqlValue = "";
			if( the_value == null )
			{
				sqlValue = "NULL";
			}
			else if ( the_value is int)
			{
				sqlValue = String.Format("{0}", the_value);
			}
			else if ( the_value is long)
			{
				sqlValue = String.Format("{0}", the_value);
			}
			else if (the_value is float)
			{
				sqlValue = String.Format("{0}", the_value.ToString());
			}
			else if (the_value is double)
			{
				sqlValue = String.Format("{0}", the_value.ToString());
			}
			else if (the_value is string)
			{
				sqlValue = String.Format("'{0}'", this.escapeString(the_value.ToString()));
			}
			else if( the_value is DateTime )
			{
				if( this.db_type == "sqlite" || this.db_type == "sqlite3" )
				{
					string the_date = ((DateTime)the_value).ToString("yyyy-MM-dd HH:mm:ss");
					sqlValue = String.Format("'{0}'", the_date);
				}
				else if( this.db_type == "sql_server" )
				{
					sqlValue = String.Format("'{0}'", ((DateTime)the_value).ToString("yyyyMMdd HH:mm:ss"));
				}
				else
				{
					sqlValue = String.Format("'{0}'", ((DateTime)the_value).ToString(this.datetime_format));
				}     
			}
			else
			{
				sqlValue = String.Format("'{0}'", this.escapeString(the_value.ToString()));
			}
			
			return sqlValue;
		}
        public virtual int Insert(string table, object data)
        {
            string columns 	= "";
            string values 	= "";
			var dataType 	= data.GetType();
			
			if (dataType == typeof(Hashtable) )
			{
				foreach (string col in (data as Hashtable).Keys)
				{
					//build columns
					if( this.db_type == "mysql" )
						columns += String.Format("`{0}`,", col);
					else if( this.db_type == "sqlite" || this.db_type == "sqlite3")
						columns += String.Format("[{0}],", col);
					else
						columns += String.Format("{0},", col);
					//build values
					values += this.FormatValue((data as Hashtable)[col]) + ",";
				}	
			}
			else if( dataType.BaseType == typeof(Entity) )
			{
				FieldInfo[] fields			= data.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (var col in fields)
				{
					object[] attributes = col.GetCustomAttributes(true);
					if ( attributes.Length <= 0 )
						continue;
					if (attributes[0].GetType() != typeof(EntityAttribute))
						continue;
					EntityAttribute attr = (EntityAttribute)attributes[0];
					//build columns
					if( this.db_type == "mysql" )
						columns += String.Format("`{0}`,", col.Name);
					else if( this.db_type == "sqlite" || this.db_type == "sqlite3")
						columns += String.Format("[{0}],", col.Name);
					else
						columns += String.Format("{0},", col.Name);
					//build values
					object the_value = col.GetValue(data);
					//	(col as FieldInfo).GetValue(data);
					values += attr.PrimaryKey ? "NULL," : this.FormatValue(the_value) + ",";
				}
			}
			else
			{
				PropertyInfo[] properties	= data.GetType().GetProperties();	
				if (properties.Length > 0)
				{
					foreach (var col in properties)
					{
						//build columns
						if (this.db_type == "mysql")
							columns += String.Format("`{0}`,", col.Name);
						else if (this.db_type == "sqlite" || this.db_type == "sqlite3")
							columns += String.Format("[{0}],", col.Name);
						else
							columns += String.Format("{0},", col.Name);
						//build values
						object the_value = col.GetValue(data, null);
						//	(col as FieldInfo).GetValue(data);
						values += this.FormatValue(the_value) + ",";
					}
				}
				
			}
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);

            string query = String.Format("INSERT INTO {0}({1}) VALUES({2})", table, columns, values);
            int res = 0;
            try
            {
                this.LastQuery = query;
                res = this.Execute(query);
            }
            catch(DataException dbex)
            {
                throw new Exception("DATABASE ERROR: " + dbex.Message + ", QUERY WAS: " + query);
            }
            catch(Exception ex)
            {
				string error = "ERROR: " + ex.Message + ", QUERY WAS: " + query;
                Console.WriteLine(ex.StackTrace);
				throw new Exception(error);
            }
            return res;
        }
		public int update(string table, Hashtable data, Hashtable wheres)
		{
			return this.Update(table, data, wheres);	
		}
        /// <summary>
        /// Update the specified table, using anonymous objects as parameters
        /// </summary>
        /// <param name="table">Table.</param>
        /// <param name="data">Data.</param>
        /// <param name="wheres">Wheres.</param>
		public int Update (string table, object data, object wheres)
		{
			string query = "UPDATE " + table + " SET ";
			var dataType = data.GetType();
			if (dataType == typeof(Hashtable))
			{
				foreach(string col in (data as Hashtable).Keys)
				{
					query += string.Format("{0} = {1},", col, this.FormatValue((data as Hashtable)[col]));
				}
			}
			else if (dataType.BaseType == typeof(Entity))
			{
				var fields = data.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (var field in fields)
				{
					object[] attributes = field.GetCustomAttributes(true);
					if (attributes.Length <= 0)
						continue;
					if (attributes[0].GetType() != typeof(EntityAttribute))
						continue;
					EntityAttribute attr = (EntityAttribute)attributes[0];
					
					object the_value = field.GetValue(data);
					query += string.Format("{0} = {1},", field.Name, this.FormatValue(the_value));
				}
			}
			else
			{
				var properties = data.GetType().GetProperties();
				if (properties.Length > 0)
				{
					foreach (PropertyInfo col in properties)
					{
						object the_value = col.GetValue(data, null);
						query += string.Format("{0} = {1},", col.Name, this.FormatValue(the_value));
					}
				}
			}
			query = query.Substring (0, query.Length - 1);
			query += " WHERE ";
			
			//build where
			if (wheres.GetType() == typeof(Hashtable))
			{
				foreach(string col in (wheres as Hashtable).Keys)
				{
					query += string.Format("{0} = {1} AND ", col, this.FormatValue((wheres as Hashtable)[col]));
				}
			}
			else
			{
				foreach (PropertyInfo col in wheres.GetType ().GetProperties ())
				{
					object the_value = col.GetValue (wheres, null);
					query += string.Format("{0} = {1} AND ", col.Name, this.FormatValue(the_value));
				}
			}
			
			query = query.Substring (0, query.LastIndexOf ("AND"));
            this.LastQuery = query;
			
			return this.Execute(query);
			
		}
		public bool delete (string table, Hashtable wheres)
		{
			string query = "DELETE FROM {0} WHERE {1}";
			string w = "";
			foreach (string col in wheres.Keys) 
			{
				w += string.Format("{0} = {1} AND ", col, this.FormatValue(wheres[col]));
			}
			w = w.Substring (0, w.Length - 4);
			query = String.Format (query, table, w);
			this.Execute (query);
			return true;
		}
        public bool delete (string table, object wheres)
        {
            string query = string.Format("DELETE FROM {0} WHERE ", table);
            try
            {
				if( wheres.GetType() == typeof(Hashtable) )
				{
					foreach(PropertyInfo col in wheres.GetType().GetProperties())
					{
						query += string.Format("{0} = {1} AND ", col.GetValue (wheres, null));
					}
				}
				else
				{
					FieldInfo[] fields = wheres.GetType().GetFields ();
					foreach (FieldInfo col in fields)
					{
						query += string.Format("{0} = {1} AND ", col.GetValue (wheres));
					}
				}
                query = query.Substring (0, query.Length - 5);
                this.LastQuery = query;
                this.Execute (query);
            }
            catch(DataException dbex)
            {
                throw new Exception("DATABASE ERROR: " + dbex.Message + ", QUERY WAS: " + query);
            }
            catch(Exception ex)
            {
                throw new Exception("ERROR: " + ex.Message + ", QUERY WAS: " + query);
            }
            return true;
        }
		/// <summary>
		/// Obtiene un registro de la tabla basada en codiciones
		/// (solo se soportan condiciones AND)
		/// </summary>
		/// <param name="table">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="wheres">
		/// A <see cref="Hashtable"/>
		/// </param>
		/// <returns>
		/// A <see cref="Hashtable"/>
		/// </returns>
		public Hashtable getRow(string table, Hashtable wheres)
		{
			string query = "SELECT * FROM {0} WHERE {1}";
			string w = "";
			foreach(string col in wheres.Keys)
			{
				w += string.Format("{0} = {1} AND ", col, this.FormatValue(wheres[col]));
			}
			w = w.Substring(0, w.Length - 4);
			query = String.Format(query, table, w);
			return this.QueryRow(query);
		}
        public SBDatabase Select(string columns)
        {
            this._builtQuery += "SELECT " + columns + " ";
            return this;
        }
        public SBDatabase From(string tables)
        {
            this._builtQuery += "FROM " + tables + " ";
            return this;
        }
        public SBDatabase From(object tables)
        {
            this._builtQuery += "FROM ";
            foreach (PropertyInfo col in tables.GetType().GetProperties())
            {
                string table = col.GetValue(tables, null).ToString();
                this._builtQuery += table + ",";
            }
            this._builtQuery = this._builtQuery.Substring(0, this._builtQuery.Length - 1) + " ";
            return this;
        }
        public SBDatabase Where(string where)
        {
            this._builtQuery += "WHERE " + where + " ";
            return this;
        }
        public SBDatabase And(string and)
        {
            this._builtQuery += "AND " + and + " ";
            return this;
        }
        public SBDatabase Or(string or)
        {
            this._builtQuery += "OR " + or + " ";
            return this;
        }
		
		public virtual IDataReader GetReader(string query)
		{
			throw new Exception("GetReader method not implemented");
			return null;
		}
		protected virtual void SetFieldValue(object obj, FieldInfo field, object _value)
		{
			try
			{
				var dataField = obj.GetType().GetField("Data", BindingFlags.Public | BindingFlags.Instance);
				if (field.FieldType == typeof(int) && _value.GetType() == field.FieldType)
					field.SetValue(obj, Convert.ToInt32(_value));
				else if (field.FieldType == typeof(long) && field.FieldType == _value.GetType())
					field.SetValue(obj, Convert.ToInt64(_value));
				else if (field.FieldType == typeof(Int64) && field.FieldType == _value.GetType())
					field.SetValue(obj, Convert.ToInt64(_value));
				else if (field.FieldType == typeof(float))
				{
					field.SetValue(obj, Convert.ToSingle(_value));
				}
				else if (field.FieldType == typeof(double))
				{
					field.SetValue(obj, Convert.ToDouble(_value));
				}
				else if (field.FieldType == typeof(DateTime) && field.FieldType == _value.GetType())
				{
					field.SetValue(obj, _value != null ? Convert.ToDateTime(_value) : DateTime.MinValue);
				}
				else if (field.FieldType == typeof(string) && field.FieldType == _value.GetType())
					field.SetValue(obj, _value.ToString());
				else if (field.FieldType == typeof(object) && field.FieldType == _value.GetType())
				{
					field.SetValue(obj, _value);
				}
				/*
				else if( obj.GetType().GetField("Meta") && obj.GetType().GetField("Meta").GetType() == typeof(ArrayList) ) 
				{
					obj.GetType().GetField("Meta").GetType().GetMethod("Add").Invoke(obj, new object[]{});
				}
				*/
				else if( dataField != null && dataField.FieldType == typeof(Hashtable) ) 
				{
					var objData = (Hashtable)dataField.GetValue(obj);
					if (objData.ContainsKey(field.Name))
						objData[field.Name] = _value;
					else
						objData.Add(field.Name, _value);
				}
				else
				{
					field.SetValue(obj, _value);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(string.Format("Unable to set field value. Field: {0}, Value: {1}", field.Name, _value));
			}
		}
		public virtual T GetRow<T>(string query)
		{
			object row;
			if (string.IsNullOrEmpty(query))
				return default(T);

			var reader 		= this.GetReader(query);
			
			lock (reader)
			{
				using (reader)
				{
					var schema = reader.GetSchemaTable();		
					reader.Read();
					row = Activator.CreateInstance(typeof(T));
					var objType = row.GetType();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						string 	columnName 	= reader.GetName(i);
						object	columnValue	= reader.GetValue(i);
						
						if ( objType == typeof(Hashtable) )
						{
							(row as Hashtable).Add(columnName, columnValue);
							//objType.GetMethod("Add").Invoke(row, new object[]{columnName, columnValue});
							//var hashtable = (Hashtable)
						}
						else
						{
							//##get object field
							var field = objType.GetField(columnName, BindingFlags.Public | BindingFlags.Instance);
							if ( field != null )
							{
								this.SetFieldValue(row, field, columnValue);
							}
						}
					}
					
				}
			}
			return (T)row;
		}
		public virtual ArrayList GetResults(string query, Type objectType = null)
		{
			ArrayList items = new ArrayList();
			var reader 		= this.GetReader(query);
			lock (reader)
			{
				using (reader)
				{
					var schema = reader.GetSchemaTable();		
					while ( reader.Read() )
					{
						var row = objectType == null ? new Hashtable() : Activator.CreateInstance(objectType);
						var type = row.GetType();
						for (int i = 0; i < reader.FieldCount; i++)
						{
							try
							{
								var theValue = reader.GetValue(i);
								if ( type == typeof(Hashtable) )
								{
									type.GetMethod("Add").Invoke(row, new object[]{reader.GetName(i), theValue});
								}
								else if (type.GetField(reader.GetName(i), BindingFlags.Public | BindingFlags.Instance) != null)
								{
									var prop = type.GetField(reader.GetName(i), BindingFlags.Public | BindingFlags.Instance);
									this.SetFieldValue(row, prop, theValue);
								}
							}
							catch(Exception e)
							{
								if ( type == typeof(Hashtable) )
									type.GetMethod("Add").Invoke(row, new object[]{reader.GetName(i), null});
								else if (type.GetField(reader.GetName(i), BindingFlags.Public | BindingFlags.Instance) != null)
								{
									var prop = type.GetField(reader.GetName(i), BindingFlags.Public | BindingFlags.Instance);
									this.SetFieldValue(row, prop, null);
								}
									
							}
							
						}
						items.Add(row);
					}
				}
			}

			return items;
		}
	}
}

