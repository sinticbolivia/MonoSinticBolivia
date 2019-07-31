using System;
using System.Collections;
using SinticBolivia.Database;
using SinticBolivia.Database.Tables;

namespace SinticBolivia
{
	public class SBPerson : SBDBObject
	{
		protected SBTablePerson db_table;
		public Hashtable attributes;
		protected SBUser user;
		
        public int PersonId
        {
            get{return this.GetInt("person_id");}
        }
		public string PersonCode
		{
			get{return this.GetString("person_code");}
			set{this.Set("person_code", value);}
		}
		public int PersonTypeId
		{
			get{return this.GetInt("person_type_id");}
			set{this.Set("person_type_id", value);}
		}
		public string PersonType
		{
			get
			{
				string query = "SELECT * FROM person_type WHERE person_type_id = " + this.PersonTypeId.ToString();
				Hashtable row = SBFactory.getDbh().QueryRow(query);
				if( row == null )
					return String.Empty;
				return row["person_type"].ToString();
			}
		}
		public string FirstName
		{
			get{return this.GetString("first_name");}
			set{this.Set("first_name", value);}
		}
		public string LastName
		{
			get{return this.GetString("last_name");}
			set{this.Set("last_name", value);}
		}
        public string MothersMaidenName
        {
            get{return this.GetString("mothers_maiden_name");}
            set{this.Set("mothers_maiden_name", value);}
        }
        public string MarriedName
        {
            get{return this.GetString("married_name");}
            set{this.Set("married_name", value);}
        }
		public DateTime BirthDay
		{
			get
            {
                return this.GetDate("birthday");
            }
			set{this.Set("birthday", value);}
		}
		public string IdentityDocument
		{
			get{return this.GetString("identity_document");}
			set{this.Set("identity_document", value);}
		}
        public string NitRucNif
        {
            get{return this.GetString("nit_ruc_nif");}
            set{this.Set("nit_ruc_nif", value);}
        }
        public string Gender
        {
            get{return this.GetString("gender");}
            set{this.Set("gender", value);}
        }
        public string Age
        {
            get{return this.GetString("age");}
            set{this.Set("age", value);}
        }
		public string Address
		{
			get{return this.GetString("address");}
			set{this.Set("address", value);}
		}
		public string Telephone
		{
			get{return this.GetString("telephone_1");}
			set{this.Set("telephone_1", value);}
		}
        public string MobileTelephone
        {
            get{return this.GetString("mobile_telephone");}
            set{this.Set("mobile_telephone", value);}
        }
		public string Email
		{
			get{return this.GetString("email");}
			set{this.Set("email", value);}
		}
		public string Status
		{
			get{return this.GetString("status");}
			set{this.Set("status", value);}
		}
		public string Image
		{
			get
			{
				string query = "SELECT * FROM object_attachments WHERE object_id = '{0}' AND lower(attachment_type) = 'image' AND object_type = 'Person' LIMIT 1";
				query = String.Format(query, this.PersonCode);
				Hashtable irow = SBFactory.getDbh().QueryRow(query);
				if( irow == null )
					return String.Empty;
				
				return irow["attachment_file"].ToString();
			}
		}
		
		public SBPerson (object code_id = null) : base()
		{
			this.db_table = new SBTablePerson();
			this.attributes = new Hashtable();
            if (code_id != null)
                this.GetDbData(code_id);
		}
        public override void GetDbData(object person_code_id)
        {
            Hashtable row = (person_code_id is int) ? this.db_table.getRow(String.Format("person_id = {0}", person_code_id)) : 
                                                        this.db_table.getRow(String.Format("person_code = '{0}'", person_code_id));
            if( row == null || row.Count <= 0)
                return;
            this._dbData = row;
            //this.getAttributes(person_code);
            this.GetDbMeta();
        }
		public void getDbData(string person_code)
		{
            this.GetDbData(person_code);
		}
		public void updateData()
		{
			this.db_table.updateRow(this._dbData);
		}
        public void GetDbMeta()
        {

        }
		public void getAttributes(string person_code)
		{
            /*
			string query = "SELECT attribute_id,attribute_name, attribute_value, creation_date FROM person_attributes WHERE person_code = '{0}' ";
			query = String.Format(query, person_code);
			
			ArrayList rows = SBFactory.getDbh().Query(query);
			if( rows == null )
				return;
			foreach(Hashtable attr in rows)
			{
				Console.WriteLine("[{0}] => {1}", attr["attribute_name"], attr["attribute_value"]);
				this.attributes.Add(attr["attribute_name"], attr["attribute_value"]);
			}
            */         
			
		}
		public static int getPersonTypeByName(string name)
		{
			string query = "SELECT person_type_id FROM person_type WHERE LOWER(person_type) = '"+name.ToLower()+"'";
			Hashtable row = SBFactory.getDbh().QueryRow(query);
			if( row == null )
				return -1;
			return int.Parse(row["person_type_id"].ToString());
		}
        public static bool CodeExists(string code)
        {
            string query = string.Format("SELECT person_code FROM person WHERE person_code = '{0}'", code);
            Hashtable row = SBFactory.getDbh().QueryRow(query);
            if( row == null || row.Count <= 0 )
                return false;
            return true;
        }
        public static string BuildNewCode(string prefix)
        {
            bool exists = true;
            string code = "";
            while (exists)
            {
                code = prefix + SBObject.generateCode();
                exists = SBPerson.CodeExists(code);
            }

            return code;
        }
        public static bool IdentityDocumentExists(int id)
        {
            string query = string.Format("SELECT person_code FROM person WHERE identity_document = {0}", id);
            if(SBFactory.getDbh().QueryRow(query) == null )
                return false;

            return true;
        }
        public object GetMeta(string meta_key)
        {
            return SBMeta.GetMeta("person_meta", "person_code_id", this.PersonCode, meta_key);
        }
        public string GetMetaString(string meta_key)
        {
            return SBMeta.GetMetaString("person_meta", "person_code_id", this.PersonCode, meta_key);
        }
        public DateTime GetMetaDate(string meta_key)
        {
            return SBMeta.GetMetaDate("person_meta", "person_code_id", this.PersonCode, meta_key);
        }
	}
}

