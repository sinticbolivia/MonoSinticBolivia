using System;
using SinticBolivia;
using SinticBolivia.Database;

namespace SinticBolivia.Database.Tables
{
	public class SBTableProductLines : SBTable
	{
		protected int line_id;
		protected string line_name;
		protected string line_description;
		protected DateTime last_modification_date;
		protected DateTime creation_date;

		public SBTableProductLines () : base("product_lines", "line_id")
		{
		}
	}
}

