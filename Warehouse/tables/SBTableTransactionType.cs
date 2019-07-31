using System;
using SinticBolivia;
using SinticBolivia.Database;

namespace SinticBolivia.Database.Tables
{
	public class SBTableTransactionType : SBTable
	{
		protected int transaction_type_id;
		protected string transaction_key;
		protected string transaction_name;
		protected string transaction_description;
		protected string in_out;
		protected DateTime last_modification_date;
		protected DateTime creation_date;

		public SBTableTransactionType () :  base("transaction_types", "transaction_type_id")
		{
		}
	}
}

