using System;
using SinticBolivia;
using SinticBolivia.Database;

namespace SinticBolivia.Database.Tables
{
	public class SBTableParameters : SBTable
	{
		protected long id;
		protected string key;
		protected string value;
		protected DateTime creation_date;

		public SBTableParameters () : base("parameters", "id")
		{
		}
	}
}

