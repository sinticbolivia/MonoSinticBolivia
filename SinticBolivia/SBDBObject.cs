using System;
using System.Collections;

namespace SinticBolivia
{
	public abstract class SBDBObject : SBObject
	{
		protected Hashtable _dbData;
        protected Hashtable _meta;

		public SBDBObject () : base()
		{
			this._dbData = new Hashtable();
            this._meta = new Hashtable();
		}
        public abstract void GetDbData(object code_id);
		public virtual void SetDbData(Hashtable data)
		{
			this._dbData = data;
		}
		public object Get(string column)
		{
			if( !this._dbData.ContainsKey(column) )
				return null;
			if (this._dbData[column].GetType() == typeof(DBNull))
				return null;
			
            if( string.IsNullOrEmpty(this._dbData[column].ToString()) )
                return null;
			return this._dbData[column];
		}
		public int GetInt(string column)
		{
			return (this.Get(column) != null ) ? Convert.ToInt32(this.Get(column)) : 0;
		}
		public long GetLong(string column)
		{
			return (this.Get(column) != null ) ? Convert.ToInt64(this.Get(column)) : 0;
		}
        public float GetSingle(string column)
        {
            return (this.Get(column) != null ) ? Convert.ToSingle(this.Get(column)) : 0f;
        }
		public double GetDouble(string column)
		{
			return (this.Get(column) != null ) ? Convert.ToDouble(this.Get(column)) : 0;
		}
		public string GetString(string column)
		{
			return (this.Get(column) != null ) ? this.Get(column).ToString() : "";
		}
		public DateTime GetDateTime(string column)
		{
            DateTime date;

            if( this.Get(column) == null )
                return DateTime.MinValue;

            if( !DateTime.TryParse(this.Get (column).ToString(), out date) )
                return DateTime.MinValue;
			return date;
		}
        public DateTime GetDate(string column)
        {
            DateTime date;

            if( this.Get(column) == null )
                return DateTime.Today;

            if( !DateTime.TryParse(this.Get (column).ToString(), out date) )
                return DateTime.Today;
            return date;
        }
		public bool Set(string column, object the_value)
		{
			if( this._dbData.ContainsKey(column) )
				this._dbData[column] = the_value;
			else
				this._dbData.Add(column, the_value);
			return true;
		}
        public object GetMeta(string meta_key)
        {
            if( !this._meta.ContainsKey(meta_key) )
                return null;
            if( string.IsNullOrEmpty(this._meta[meta_key].ToString()) )
                return null;
            return this._meta[meta_key];
        }
	}
}

