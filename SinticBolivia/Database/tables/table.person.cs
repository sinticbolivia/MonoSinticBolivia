using System;

namespace SinticBolivia.Database.Tables
{
	public class SBTablePerson : SBTable
	{
        protected int person_id;
		protected string person_code;
		protected int person_type_id;
		protected string first_name;
		protected string last_name;
        protected string mothers_maiden_name;
        protected string married_name;
		protected string identity_document;
        protected string gender;
        protected int age;
        protected DateTime birthday;
		protected string address;
        protected string city;
        protected string country;
        protected string zip;
		protected string telephone_1;
        protected string telephone_2;
        protected string mobile_telephone;
		protected string email;
        protected string nit_ruc_nif;
        protected int author_id;
		protected string status;
        protected DateTime last_modification_date;
		protected DateTime creation_date;
		
		public SBTablePerson () : base("person", "person_code")
		{
			
		}
	}
}

