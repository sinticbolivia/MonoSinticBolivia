using System;
using SinticBolivia;
using SinticBolivia.Database;

namespace SinticBolivia.Database.Tables
{
	public class SBTableStores : SBTable
	{
		protected int store_id;
		protected string store_name;
		protected string store_address;
		protected DateTime last_modification_date;
		protected DateTime creation_date;

		public SBTableStores () : base("stores", "store_id")
		{
		}
	}
}

