using System;
using System.Collections;
using SinticBolivia.Database;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace SinticBolivia.Database
{
	public class SqlServer : SBDatabase
	{
        protected SqlConnection con = null;
        protected SqlDataReader reader;

		public SqlServer (string conString)
		{
            this.connectionString = conString;
			this.db_type = "sql_server";
		}
        public override bool open()
		{
            if( this.con == null )
                this.con = new SqlConnection(this.connectionString);
            this.con.Open();
			return true;
		}
		public override bool close()
		{
            if (this.con == null)
                return false;
            this.con.Close();
            this.con.Dispose();
            this.con = null;
			return true;
		}
        public SqlDataReader QueryDatareader(string query)
        {
            SqlCommand cmd = new SqlCommand(query, this.con);
            SqlDataReader reader = null;
            this.LastQuery = query;
			try
			{
            	reader = cmd.ExecuteReader();
			}
			catch(SqlException ex)
			{
                if(reader != null && !reader.IsClosed) reader.Close();
				throw new Exception("MSSQL ERROR: " + ex.Message + ", QUERY WAS: " + query);
			}
            return (SqlDataReader)reader;
        }
		public override ArrayList Query(string query)
		{
            ArrayList rows = null;

            //using(this.con)
            //{
                //this.reader = this.QueryDatareader(query);
                SqlDataReader reader = this.QueryDatareader(query);
                if (!reader.HasRows)
                {
                    reader.Close();
                    return rows;
                }
                rows = new ArrayList();
                while (reader.Read())
                {
                    Hashtable row = new Hashtable();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    rows.Add(row);
                }
                reader.Close();
                //this.close();
            //}

            return rows;
		}
		public override ArrayList ExecuteProcedure(string procedureName, Hashtable procParameters)
		{
            ArrayList rows = new ArrayList();
            //using(this.con)
            //{
                SqlCommand cmd = new SqlCommand(procedureName, this.con);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (object param_name in procParameters.Keys)
                {
                    cmd.Parameters.AddWithValue("@" + param_name, procParameters[param_name]);
                    //cmd.Parameters["@" + key] = data[key];
                }
                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Hashtable row = new Hashtable();
                        for (int i = 0; i < this.reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        rows.Add(row);
                    }
                    reader.Close();
                }
                catch(SqlException ex)
                {
                    throw new Exception("MSSQL ERROR: " + ex.Message + ", PROCEDURE: " + procedureName + ", COMMAND:" + cmd.CommandText);
                }
            //}

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
                cols += string.Format("[{0}],", col);
            }
            cols = cols.Substring(0, cols.Length - 1);
            
            return cols;
        }
		public override int Execute(string query)
		{
            int res = 0;
            //using(this.con)
            //{
                try
                {
                    SqlCommand cmd = new SqlCommand(query, this.con);
                    
                    if( query.IndexOf("INSERT", StringComparison.InvariantCultureIgnoreCase) != -1 )
                    {
                        query += ";SELECT CAST(scope_identity() AS int) AS ID";
                        cmd.CommandText = query;
                        res = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        res = cmd.ExecuteNonQuery();
                    }
                }
                catch(SqlException ex)
                {
                    throw new Exception("MSSQL ERROR: " + ex.Message + ", QUERY WAS: " + query);
                }
                //this.close();
            //}

            return res;
		}
		public ArrayList queryPage(string table, string columns, string order_by, int index, int rows_per_page)
		{
			/*
			string query = "SELECT * "+
							"FROM ("+
								"SELECT ROW_NUMBER() OVER(ORDER BY a.nombre) AS num,"+
								"a.nombre, a.codestructura,a.codtipoestructura" +
								"FROM estructuraempresa AS a"+
		
							") AS sub"+
							"WHERE (num > 1 AND num <= (1 + 5))";
			*/
			string query = "SELECT * "+
							"FROM ("+
								"SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS num,"+
								"{1}" +
								"FROM estructuraempresa AS tt"+
		
							") AS sub"+
							"WHERE (num > {2} AND num <= ({3}))";
			string[] cols = columns.Split(',');
			string prefix_cols = "";
			foreach(string col in cols)
			{
				prefix_cols += "tt." + col + ",";
			}
			prefix_cols = prefix_cols.Substring(0, prefix_cols.Length - 1);
			query = String.Format(query, "tt." + order_by, prefix_cols, index, (index + rows_per_page));
			
			return this.Query(query);
		}
		public override string escapeString(string str)
		{
			return str.Replace("'", "\'");
		}
        public override bool CreateTable(string table_name, object cols, string[] primary_key, bool auto_increment = true)
        {
            return true;
        }
	}
}

