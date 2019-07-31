using System;
using System.Collections;

namespace SinticBolivia
{
    public class SBMeta
    {
        public SBMeta()
        {
        }
        public static void AddMeta(string table, string table_column_key, object column_key_value, string meta_key, object meta_value)
        {
            Hashtable data = new Hashtable();
            data.Add(table_column_key, column_key_value);
            data.Add("meta_key", meta_key);
            data.Add("meta_value", meta_value);
            data.Add("last_modification_date", DateTime.Now);
            data.Add("creation_date", DateTime.Now);
            SBFactory.getDbh().insert(table, data);
        }
        public static object GetMeta(string table, string table_column_key, object column_key_value, string meta_key, bool single = true)
        {
            string query = string.Format("SELECT * FROM {0} WHERE {1} = '{2}' AND meta_key = '{3}'", 
                                         table, table_column_key, column_key_value, meta_key);
            if (single)
            {
                Hashtable row = SBFactory.getDbh().QueryRow(query);
                if (row == null)
                    return null;
                return row ["meta_value"];
            } 
            else
            {
                ArrayList rows = SBFactory.getDbh().Query(query);
                if( rows == null )
                    return null;
                return rows;
            }
        }
        public static string GetMetaString(string table, string table_column_key, object column_key_value, string meta_key)
        {
            object res = SBMeta.GetMeta(table, table_column_key, column_key_value, meta_key);
            if( res == null )
                return "";
            return res.ToString();
        }
        public static DateTime GetMetaDate(string table, string table_column_key, object column_key_value, string meta_key)
        {
            object meta_value = SBMeta.GetMeta(table, table_column_key, column_key_value, meta_key);
            if( meta_value == null )
                return DateTime.MinValue;
            DateTime date;
            if( !DateTime.TryParse(meta_value.ToString(), out date) )
                return DateTime.MinValue;
            return date;
        }
        public static bool UpdateMeta(string table, string table_column_key, object column_key_value, string meta_key, object meta_value)
        {
            if (SBMeta.GetMeta(table, table_column_key, column_key_value, meta_key) == null)
            {
                SBMeta.AddMeta(table, table_column_key, column_key_value, meta_key, meta_value);
            }
            else
            {
                Hashtable data = new Hashtable();
                data.Add("meta_value", meta_value);
                Hashtable w = new Hashtable();
                w.Add(table_column_key, column_key_value);
                w.Add("meta_key", meta_key);
                SBFactory.getDbh().update(table, data, w);
            }

            return true;
        }
    }
}

