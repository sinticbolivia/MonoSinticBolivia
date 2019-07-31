using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Reflection;
//#if __MonoCS__
using Mono.Data.Sqlite;
//#endif
namespace SinticBolivia.Database
{
	public class SQLite : SBDatabase
	{
		/*
		[2.0 profile in the new assembly]
		Data Source=file:/path/to/file
		Data Source=|DataDirectory|filename
		*/
		protected string 			db_path;
		protected IDbConnection 	con;
		protected SqliteDataReader 	reader = null;
		
		
		public SQLite (string db_path)
		{
			this.db_type = "sqlite";
			this.db_path = db_path;
		}
		public override bool open()
		{
			
			SqliteConnection.SetConfig(SQLiteConfig.Serialized);
			this.con = (IDbConnection)new SqliteConnection(this.db_path);
			this.con.Open();
			return true;
			
		}
		public override bool close()
		{
			this.con.Close();
			return true;
		}
		public override ArrayList Query(string query = "")
		{
            if (string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(this._builtQuery))
                query = this._builtQuery;

			if( query[query.Length - 1] != ';' )
				query += ";";
			
			ArrayList rows 		= new ArrayList();
			SqliteCommand cmd 	= new SqliteCommand(query, (SqliteConnection)this.con);
			
			lock (this.con)
			{
				try
				{
					this.LastQuery = query;
					if( this.reader != null && !this.reader.IsClosed )
					{
						this.reader.Close();
					}

					using( this.reader = cmd.ExecuteReader() )
					{
						if (!this.reader.HasRows)
						{
							this.reader.Close();
							cmd.Dispose();
							return rows;
						}
						var schema = this.reader.GetSchemaTable();		
						while ( this.reader.Read() )
						{
							Hashtable row = new Hashtable();
							for (int i = 0; i < reader.FieldCount; i++)
							{
								/*
								if(schema.Columns[i].DataType == typeof(DateTime) )
								{
									DateTime datetime = DateTime.Today;
									if( DateTime.TryParse(reader.GetString(i), out datetime) )
									{
										
									}
									row.Add(reader.GetName(i), datetime);
								}
								else
								{
									row.Add(reader.GetName(i), reader.GetValue(i));
								}
		                        */
								try
								{
									row.Add(reader.GetName(i), reader.GetValue(i));
								}
								catch(Exception e)
								{
									row.Add(reader.GetName(i), null);
								}
							}
							rows.Add(row);
						}
						
						reader.Close();
						
					}
					cmd.Dispose();
					cmd = null;
				}
				catch(DataException ex)
				{
					throw new Exception("SQLite DataException ERROR: " + ex.Message + ", QUERY WAS: " + 
						query + ", STACK: " + ex.StackTrace);
				}
				catch(Exception ex)
				{
					throw new Exception("SQLite ERROR: " + ex.Message + ", QUERY WAS: " + query + ", STACK:: " + ex.StackTrace);
				}
				
			}
			return rows;
		}
		public override string escapeString (string str)
		{
			return str.Replace("'", "\''");
		}
		public override ArrayList ExecuteProcedure(string procedureName, Hashtable procParameters)
		{
			return new ArrayList();
		}
        public override void BeginTransaction()
        {
            if (this._transactionOpen)
                return;
            //start transaction
            SqliteCommand cmd = new SqliteCommand("begin", (SqliteConnection)this.con);
            cmd.ExecuteNonQuery();
            this._transactionOpen = true;
        }
        public override void EndTransaction()
        {
            //end transaction
            SqliteCommand cmd = new SqliteCommand("end", (SqliteConnection)this.con);
            cmd.ExecuteNonQuery();
            this._transactionOpen = false;
        }
        public override string PrepareColumns(string[] columns)
        {
            string cols = "";
            foreach(string col in columns)
            {
                cols += string.Format("[{0}],", col);
            }
            cols = cols.Substring(0, cols.Length - 1);

            return cols;
        }
		public override int Execute(string query)
		{
            int res = 0;
            try
            {
                this.LastQuery = query;
                SqliteCommand cmd = new SqliteCommand(query, (SqliteConnection)this.con);
                res = cmd.ExecuteNonQuery();
				Console.WriteLine("EXCEUTE: {0}", query);
                //get last insert id
                if( query.IndexOf("insert", StringComparison.InvariantCultureIgnoreCase) != -1 )
                {
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
			catch(DataException ex)
            {
                throw new Exception("SQLite ERROR: " + ex.Message + ", QUERY WAS: " + query);
            }
			/*
			catch(Exception ex)
			{
				throw new Exception("SQLite ERROR: " + ex.Message + ", QUERY WAS: " + query);
			}
			*/
			return res;
		}
        public override bool CreateTable(string table_name, object cols, string[] primary_key = null, bool auto_increment = true)
        {
            string query = "CREATE TABLE IF NOT EXISTS " + table_name + "({0});";
            string _cols = "";
            foreach (PropertyInfo col in cols.GetType().GetProperties())
            {
                _cols += string.Format("[{0}] {1},", col.Name, col.GetValue(cols, null).ToString());
            }
            _cols = _cols.Substring(0, _cols.Length - 1);
            query = string.Format(query, _cols);
            int res = this.Execute(query);

            return (res > 0) ? true : false;
        }
		public override IDataReader GetReader(string query)
		{
			if (string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(this._builtQuery))
				query = this._builtQuery;

			if( query[query.Length - 1] != ';' )
				query += ";";
			//var dataset = new DataSet();
			SqliteDataReader reader;

			lock (this.con)
			{
				try
				{
					using(var cmd 	= new SqliteCommand(query, (SqliteConnection)this.con))
					{
						reader = cmd.ExecuteReader();
						//var adapter = new SqliteDataAdapter(cmd);
						//adapter.Fill(dataset);
					}
				}
				catch(DataException ex)
				{
					throw new Exception("SQLite DataException ERROR: " + ex.Message + ", QUERY WAS: " + 
						query + ", STACK: " + ex.StackTrace);
				}
				catch(Exception ex)
				{
					throw new Exception("SQLite ERROR: " + ex.Message + ", QUERY WAS: " + query + ", STACK:: " + ex.StackTrace);
				}
			}
			return reader;
		}
	}
}

