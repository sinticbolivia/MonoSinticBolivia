using System;
using System.Collections;
using SinticBolivia;
using SinticBolivia.Warehouse;
using SinticBolivia.Warehouse.Tables;

namespace SinticBolivia.Warehouse
{
	public class SBSupplier : SBDBObject
	{
		protected SBTableSuppliers suppliers_table;
		
		//public SMPerson Person{get{return this.person;}}
		public int Id
		{
			get{return this.GetInt("supplier_id");}
            set{this.Set("supplier_id", value);}
		}
		public string Name
		{
			get{return this.GetString("supplier_name");}
            set{this.Set("supplier_name", value);}
		}
        public string Key
        {
            get{return this.GetString("supplier_key");}
            set{this.Set("supplier_key", value);}
        }
		public string Address
		{
			get{return this.GetString("supplier_address");}
            set{this.Set("supplier_address", value);}
		}
        public string Telephone1
        {
            get{return this.GetString("supplier_telephone_1");}
        }
        public string Telephone2
        {
            get{return this.GetString("supplier_telephone_2");}
        }
        public string Detaills
        {
            get{return this.GetString("supplier_details");}
        }
        public string City
        {
            get{return this.GetString("supplier_city");}
        }
        public string Email
        {
            get{return this.GetString("supplier_email");}
        }
        public string ContactPerson
        {
            get{return this.GetString("supplier_contact_person");}
        }
        public string NitRucNif
        {
            get{return this.GetString("nit_ruc_nif");}
        }
		public DateTime CreationDate
		{
			get{return this.GetDateTime("creation_date");}
		}
        public DateTime LastModificationDate
        {
            get{ return this.GetDateTime("last_modification_date");}
            set{this.Set("last_modification_date", value);}
        }
		public SBSupplier (int supplier_id = 0) : base()
		{
			this.suppliers_table = new SBTableSuppliers();
			if( supplier_id > 0 )
                this.GetDbData(supplier_id);
		}
        public override void GetDbData(object code_id)
        {
            Hashtable row = this.suppliers_table.getRow("supplier_id = " + code_id.ToString());
            if( row == null )
                return;
            this._dbData = row;
        }
		public void getData(int supplier_id)
		{
            this.GetDbData(supplier_id);			
		}
		public void updateData()
		{
			
		}
	}
}

