using System;
using SinticBolivia;
using SinticBolivia.Database;

namespace SinticBolivia.Database.Tables
{
	public class SBTableTerminals : SBTable
	{
		protected int terminal_id;
		protected int branch_id;
		protected string terminal_name;
		protected string terminal_type;
		protected string status;
		protected DateTime creation_date;
		
		public SBTableTerminals () :  base("terminals", "terminal_id")
		{
		}
	}
}

