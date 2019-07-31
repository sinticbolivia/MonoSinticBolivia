using System;
using SinticBolivia.Database;
using SinticBolivia.Database.Tables;
namespace SinticBolivia.Warehouse.Tables
{
	public class SBTableSuppliers : SBTable
	{
		protected int supplier_id;
        protected string supplier_key;
		protected string supplier_name;
		protected string supplier_address;
		protected string supplier_telephone_1;
		protected string supplier_telephone_2;
		protected string supplier_details;
		protected string supplier_city;
		protected string supplier_email;
		protected string supplier_contact_person;
		protected string nit_ruc_nif;
		protected string bank_name;
		protected string bank_account;
		protected DateTime last_modification_date;
		protected DateTime creation_date;
		
		public SBTableSuppliers () : base("suppliers", "supplier_id")
		{
		}
	}
}

