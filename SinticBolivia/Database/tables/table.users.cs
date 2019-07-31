using System;
using SinticBolivia.Database;

namespace SinticBolivia.Database.Tables
{
	public class SBTableUsers : SBTable
	{
		protected int user_id;
        protected string first_name;
        protected string last_name;
		protected string username;
		protected string pwd;
        protected string email;
		protected int role_id;
		protected string status;
		protected DateTime creation_date;
		
		public SBTableUsers () : base("users", "user_id")
		{
		}
	}
}

