using System;
using SinticBolivia.Database;

namespace SinticBolivia.Database.Tables
{
	public class SBTableRol : SBTable
	{
		protected int role_id;
		protected string role_name;
		protected string role_description;
        protected DateTime last_modification_date;
		protected DateTime creation_date;
		
		public SBTableRol () : base("user_roles", "role_id")
		{
		}
	}
}

