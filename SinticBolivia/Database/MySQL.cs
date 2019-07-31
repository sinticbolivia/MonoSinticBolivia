using System;
using System.Collections;
using System.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Globalization;

namespace SinticBolivia.Database
{
	public class MySQL : SBDatabase
	{
		protected MySqlConnection con;
		protected MySqlDataReader reader;
		/*
		//protected long last_insert_id;
		public long LastInsertID
		{
			get{return this.last_insert_id;}
			//set{this.last_insert_id = value;}
		}
		*/
		public MySQL (string conString)
		{
			this.connectionString = conString;
			this.db_type = "mysql";
		}
		public override bool open()
		{
			this.con = new MySqlConnection(this.connectionString);
			this.con.Open();
			return true;
		}
		public override bool close()
		{
			this.con.Close();
			return true;
		}
		public override ArrayList Query (string query)
		{
			MySqlCommand cmd = new MySqlCommand(query, this.con);
			ArrayList rows = new ArrayList();
            string datetime_format = CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern;
			try
			{
				this.reader = cmd.ExecuteReader();
				if( this.reader.HasRows )
				{
					while(this.reader.Read())
					{
						Hashtable row = new Hashtable();
		                for (int i = 0; i < this.reader.FieldCount; i++)
		                {
                            if( reader.GetValue(i) is MySqlDateTime)
                            {
                                if( reader.GetValue(i) == null || string.IsNullOrEmpty(reader.GetValue(i).ToString()) )
                                {
                                    row.Add(reader.GetName(i), DateTime.MinValue);
                                }
                                else
                                {
                                    //string date_str = (reader.GetValue(i) as MySqlDateTime).ToString(datetime_format);
                                    row.Add(reader.GetName(i), reader.GetValue(i));
                                }

                            }
		                    else
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
		                }
		                rows.Add(row);
					}
				}
				else
				{
					rows = null;
				}
				this.reader.Close();
				this.reader.Dispose();
			}
			catch(MySqlException ex)
			{
				if(this.reader != null && !this.reader.IsClosed)
				{
					this.reader.Close();
					this.reader.Dispose();
				}
				throw new Exception("MySQL ERROR: " + ex.Message + " Query was: " + query);
			}
			return rows;
		}
        public override void BeginTransaction()
        {
            //start transaction
        }
        public override void EndTransaction()
        {
            //end transaction
        }
        public override string PrepareColumns(string[] columns)
        {
            string cols = "";
            foreach(string col in columns)
            {
                cols += string.Format("`{0}]`,", col);
            }
            cols = cols.Substring(0, cols.Length - 1);
            
            return cols;
        }
		public override int Execute (string query)
		{
			int res = 0;
			try
			{
				MySqlCommand cmd = new MySqlCommand(query, this.con);
				res = cmd.ExecuteNonQuery();
				//set last inser id
				this.lastInserId = cmd.LastInsertedId;
			}
			catch(MySqlException ex)
			{
				throw new Exception("MySQL ERROR: " + ex.Message + " Query was: " + query);
			}
			if( query.IndexOf("insert", StringComparison.InvariantCultureIgnoreCase) != -1 )
			{
				return (int)this.lastInserId;
			}
			return res;
		}
		public override ArrayList ExecuteProcedure (string procedureName, Hashtable procParameters)
		{
			throw new NotImplementedException ();
		}
		public override string escapeString(string str)
		{
			return str.Replace("'", "\'");
		}
        public override bool CreateTable(string table_name, object cols, string[] primary_key = null, bool auto_increment = true)
        {
            return true;
        }
	}
}

