using System;
using System.Collections;
using SinticBolivia;
using SinticBolivia.Database;
using SinticBolivia.Database.Tables;

namespace SinticBolivia.Warehouse
{
    public class SBTransactionType : SBDBObject
    {
        protected SBTableTransactionType  _dbTable;
        public int Id
        {
            get{return this.GetInt("transaction_type_id");}
        }
        public string Key
        {
            get{return this.GetString("transaction_key");}
            set{this.Set("transaction_key", value);}
        }
        public string Name
        {
            get{return this.GetString("transaction_name");}
            set{this.Set("transaction_name", value);}
        }
        public string Description
        {
            get{return this.GetString("transaction_description");}
            set{this.Set("transaction_description", value);}
        }
        public string InputOutput
        {
            get{return this.GetString("in_out");}
            set{this.Set("in_out", value);}
        }
        public DateTime LastModificationDate
        {
            get{return this.GetDateTime("last_modification_date");}
            set{this.Set("last_modification_date", value);}
        }
        public DateTime CreationDate
        {
            get{return this.GetDateTime("creation_date");}
        }

        public SBTransactionType(int id = 0) : base()
        {
            this._dbTable = new SBTableTransactionType();
            if (id != 0)
                this.GetDbData(id);
        }
        /// <summary>
        /// Gets transaction type data from database
        /// </summary>
        /// <param name="code_id">Code_id.</param>
        public override void GetDbData(object code_id)
        {
            Hashtable row = this._dbTable.getRow("transaction_type_id = " + code_id.ToString());
            if (row == null)
                return;
            this._dbData = row;
        }
    }
}

