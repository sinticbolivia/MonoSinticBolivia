using System;
using SinticBolivia.Database;

namespace SinticBolivia.Warehouse.Tables
{
	public class SBTableTransactionDetails : SBTable
	{
		protected long detail_id;
		protected string transaction_code;
		
		public SBTableTransactionDetails () : base("transaction_details", "detail_id")
		{
		}
	}
}

