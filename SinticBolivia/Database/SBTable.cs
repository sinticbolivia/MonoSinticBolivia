using System;
using System.Collections;

namespace SinticBolivia.Database
{
	public class SBTable : SBObject
	{
		protected string table;
		protected string primary_key;
		protected string last_query;
		public string LastQuery{get{return this.last_query;}}
		public string TableName{get{return this.table;}}
		public long LastInsertID
		{
			get
			{
				return ((SBDatabase)SBFactory.getDbh()).LastInsertID;
			}
		}
		public SBTable (string table, string key)
		{
			this.table = table;
			this.primary_key = key;
		}
		public ArrayList getRows(string wheres)
		{
			ArrayList skip = new ArrayList();
			skip.Add("table");
			skip.Add("primary_key");
			skip.Add("Data");
			skip.Add("last_query");			
			string query = "SELECT {0} FROM {1} {2}";
			string w = (String.IsNullOrEmpty(wheres)) ? "" : String.Format("WHERE {0}", wheres);
			query = String.Format(query, this.getPropertiesWithGlue(",", skip), this.table, w);
			return SBFactory.getDbh().Query(query);
		}
		public Hashtable getRow(string wheres)
        {
            if (SBFactory.getDbh().db_type == "mysql" || SBFactory.getDbh().db_type == "sqlite" || SBFactory.getDbh().db_type == "sqlite3")
            {
                ArrayList skip = new ArrayList();
                skip.Add("table");
                skip.Add("primary_key");
                skip.Add("Data");
                skip.Add("last_query");         
                string query = "SELECT {0} FROM {1} {2} LIMIT 1";
                string w = (String.IsNullOrEmpty(wheres)) ? "" : String.Format("WHERE {0}", wheres);
                query = String.Format(query, this.getPropertiesWithGlue(",", skip), this.table, w);
                return SBFactory.getDbh().QueryRow(query);
            } 
            else
            {
                ArrayList rows = this.getRows(wheres);
                if(rows != null && rows.Count > 0)
                    return (Hashtable)rows[0];
            }
			
			return null;
		}
		public void updateRow(Hashtable row_data)
		{
			if( row_data[this.primary_key] == null )
				throw new Exception("SMTable: No primary key defined, ("+this.primary_key+")");
            object aux = row_data[this.primary_key];
			Hashtable w = new Hashtable();
			w.Add(this.primary_key, row_data[this.primary_key]);
			row_data.Remove(this.primary_key);
			SBFactory.getDbh().update(this.table, row_data, w);
            //restore primary key data
            row_data.Add(this.primary_key, aux);
		}
		public int insertRow(Hashtable new_row)
		{
			return SBFactory.getDbh().insert(this.table, new_row);
		}
		public bool DeleteRow(string column, object the_value)
		{
			Hashtable w = new Hashtable();
			w.Add(column, the_value);
			SBFactory.getDbh().delete(this.table, w);

			return true;
		}
		public void setPrimaryKey(string column_name)
		{
			this.primary_key = column_name;
		}
	}
}

