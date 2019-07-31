using System;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace SinticBolivia
{
	public class SBObject
	{
		public const string BIND_ALL = "all";
		public const string BIND_PUBLIC = "public";
		public const string BIND_NONPUBLIC = "non_public";
		public const string BIND_INSTANCE = "instance";
		private Type objType;
		private FieldInfo[] properties;
		public Hashtable Data;
		//public Hashtable Data{get{ return this.data; }set{ this.data = value;}}
		public SBObject ()
		{
			this.Data = new Hashtable();
		}
		/**
		 * Get current instance public properties
		 **/
		protected FieldInfo[] getObjectProperties()
		{
			//get current object type
			this.objType = this.GetType();
			//get object properties
			this.properties = this.objType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			
			return this.properties;
            
		}
		public Hashtable getProperties()
		{
			Hashtable properties = new Hashtable();
			
			foreach(FieldInfo prop in this.getObjectProperties())
			{
				properties.Add(prop.Name, prop.GetValue(this));
			}
			return properties;
		}
		public string getPropertiesWithGlue(string glue)
		{
			string props = "";
			foreach(FieldInfo prop in this.getObjectProperties())
			{
				props += String.Format("{0}{1}", prop.Name, glue);
			}
			if( props.LastIndexOf(glue) != -1 )
				props = props.Substring(0, props.LastIndexOf(glue));
			return props;
		}
		public string getPropertiesWithGlue(string glue, ArrayList skip_fields)
		{
			string props = "";
			foreach(FieldInfo prop in this.getObjectProperties())
			{
				if( skip_fields.Contains(prop.Name) )
					continue;
				props += String.Format("{0}{1}", prop.Name, glue);
			}
			if( props.LastIndexOf(glue) != -1 )
				props = props.Substring(0, props.LastIndexOf(glue));
			return props;
		}
		protected bool propertyExists(string propertyName)
		{
			var field = this.GetType().GetField(propertyName, BindingFlags.Public);
			if (field == null)
				return false;
			return true;
			/*
			bool exists = false;
			foreach(FieldInfo p in this.getObjectProperties())
			{
				if( p.Name.Equals(propertyName) )
				{
					exists = true;
					break;
				}
					
			}
			return exists;
			*/
		}
		
		protected FieldInfo GetField(string name)
		{
			return this.GetType().GetField(name);
		}
		protected bool setPropertyValue(string property, object new_value)
		{
			var field = this.GetField(property);
			//foreach(FieldInfo p in this.getObjectProperties())
			//{
				//if( p.Name.Equals(property) )
				//{
					DateTime date;
					/*
					if( Int32.TryParse(new_value.ToString(), out integer) )
					{
						p.SetValue(this, integer);	
					}
					else if( float.TryParse(new_value.ToString(), out dec) )
					{
						p.SetValue(this, dec);	
					}
					//else if( new_value.GetType() == typeof(DateTime) )
					else if( DateTime.TryParse(new_value.ToString(), out date))
					{
						p.SetValue(this, date);	
					}
					else
					{
						p.SetValue(this, new_value);
					}
					*/
					if( field.GetType() == typeof(int) )
					{
						field.SetValue(this, Convert.ToInt32(new_value));	
					}
					else if( field.GetType() == typeof(System.DateTime) )
					{
						field.SetValue(this, Convert.ToDateTime(new_value));	
					}
					else if( DateTime.TryParse(new_value.ToString(), out date) )
					{
						field.SetValue(this, date);	
					}
					else if( field.GetType() == typeof(string) )
					{
						field.SetValue(this, new_value.ToString() );	
					}
					else
					{
						field.SetValue(this, new_value);	
					}
					//break;
				//}		
			//}
			return true;
		}
		public void bind(Hashtable data)
		{
			this.getObjectProperties();
						
			foreach(string key in data.Keys)
			{
				if( this.propertyExists( key ) && data[key].GetType() != typeof(System.DBNull) )
				{
					this.setPropertyValue(key, data[key]);
				}
			}
		}
		public void bind(object data)
		{
			this.getObjectProperties();
			Type objType = data.GetType();
			FieldInfo[] data_properties = objType.GetFields();
			foreach(FieldInfo prop in data_properties)
			{
				if( this.propertyExists( prop.Name ) )
				{
					this.setPropertyValue(prop.Name, prop.GetValue(data));
				}
			}
		}
		public static string generateCode()
		{
			int maxSize  = 8;
			int minSize = 5 ;
			char[] chars = new char[62];
			string a;
			//a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890" + name;
			a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";// + name;
			chars = a.ToCharArray();
			int size  = maxSize;
			byte[] data = new byte[1];
			RNGCryptoServiceProvider  crypto = new RNGCryptoServiceProvider();
			crypto.GetNonZeroBytes(data) ;
			//size =  maxSize;
			data = new byte[size];
			crypto.GetNonZeroBytes(data);
			StringBuilder result = new StringBuilder(size) ;
			foreach(byte b in data )
			{ 
				result.Append(chars[b % (chars.Length - 1)]); 
			}
			return result.ToString().ToUpper();
		}
		public static bool hasAttachment(string object_id, string object_type)
		{
			string query = "SELECT attachment_id FROM object_attachments WHERE object_id = '{0}' AND object_type = '{1}'";
			query = String.Format(query, object_id, object_type);
			if(SBFactory.getDbh().Query(query) == null)
				return false;
			return true;
		}
		public void SetFieldValue(string fieldName, object _value)
		{
			//Console.WriteLine("BIND: Trying to bind FieldName: {0}, Type: {1}", fieldName, _value.GetType().ToString());
			var field = this.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
			if (field == null)
			{
				//Console.WriteLine("The field does not exists");
				return;
			}
			
			var fieldType = field.FieldType;
			
			//Console.WriteLine("BIND: FieldName: {0}, FieldType: {1}", field.Name, fieldType.ToString());
			try
			{
				if (fieldType == typeof(int))
				{
					if (_value.GetType() != typeof(int))
						field.SetValue(this, Convert.ToInt32(Convert.ChangeType(_value, typeof(int))));
					else
						field.SetValue(this, Convert.ToInt32(_value));
				}
				else if (fieldType == typeof(long))
				{
					if (_value.GetType() != typeof(long))
						field.SetValue(this, (long)Convert.ToInt64(Convert.ChangeType(_value, typeof(long))));
					else
						field.SetValue(this, (long)Convert.ToInt64(_value));
				}
				else if (fieldType == typeof(float))
				{
					if (_value.GetType() != typeof(float))
						//field.SetValue(this, Convert.ToSingle(Convert.ChangeType(_value, typeof(float))));
						field.SetValue(this, Convert.ToSingle(_value.ToString()));
					else
						field.SetValue(this, Convert.ToSingle(_value));
				}
				else if (fieldType == typeof(double))
				{
					if (_value.GetType() != typeof(double))
						//field.SetValue(this, Convert.ToDouble(Convert.ChangeType(_value, typeof(double))));
						field.SetValue(this, Convert.ToDouble(_value.ToString()));
					else
						field.SetValue(this, Convert.ToDouble(_value));
				}
				else if( fieldType == typeof(DateTime) )
					field.SetValue(this, Convert.ToDateTime(_value));
				else if( fieldType == typeof(string) )
					field.SetValue(this, _value.ToString());
				else
					field.SetValue(this, _value);	
			}
			catch(Exception e)
			{
				Console.WriteLine("BIND ERROR: {0}\nSourceField: {1}\nSourceType: {2}\nDestField: {3}\nDestType: {4}", 
					e.Message, fieldName, _value.GetType().ToString(), field.Name, field.GetType().ToString());
			}
			
		}
		public object GetFieldValue(string fieldName)
		{
			var field = this.GetField(fieldName);
			if (field == null)
				return null;
			return field.GetValue(this);
		}
		/// <summary>
		/// Bind object field with specified data
		/// 
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="BindType">Bind type.</param>
		public void Bind(object data, string BindType = SBObject.BIND_PUBLIC)
		{
			var dataType = data.GetType();
			
			if (dataType == typeof(Hashtable))
			{
				var _data = (data as Hashtable);
				foreach (string key in _data.Keys)
				{
					this.SetFieldValue(key, _data[key]);
				}
			}
			else
			{
				var fields = dataType.GetFields(BindingFlags.Public);
				foreach (FieldInfo field in fields)
				{
					this.SetFieldValue(field.Name, field.GetValue(data));
				}
			}
		}
		public Hashtable GetData()
		{
			return this.Data;
		}
	}
}

