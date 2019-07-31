using System;
using SinticBolivia.Database;

namespace SinticBolivia.Warehouse.Tables
{
	public class SBTableTransactions : SBTable
	{
        protected int transaction_id;
		protected string transaction_code;
		protected string transaction_type_id;
        protected int store_id;
        protected int user_id;
        protected string owner_code_id;
        protected string details;
		protected float sub_total;
        protected float discount;
        protected float total;
        protected int total_items;
		protected string status;
        protected int sequence;
        protected DateTime last_modification_date;
		protected DateTime creation_date;
		
		public SBTableTransactions () : base("transactions", "transaction_code")
		{
		}
	}
}

