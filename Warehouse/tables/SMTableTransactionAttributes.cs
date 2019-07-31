using System;
using SinticBolivia.Database;

namespace SinticBolivia.Warehouse.Tables
{
	public class SBTableTransactionAttributes : SBTable
	{
		protected int attribute_id;
		protected string transaction_code;
		protected string attribute_name;
		protected string attribute_value;
		protected DateTime creation_date;
		
		public SBTableTransactionAttributes () : base("transaction_attributes", "attribute_id")
		{
		}
	}
}

