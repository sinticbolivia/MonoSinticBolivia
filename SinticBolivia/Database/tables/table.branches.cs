using System;
using SinticBolivia;
using SinticBolivia.Database;

namespace SinticBolivia.Database.Tables
{
	public class SBTableBranches : SBTable
	{
		protected int branch_id;
		protected string branch_name;
		protected string branch_address;
		protected float branch_lat = 0f;
		protected float branch_lng = 0f;
		protected int company_id = 0;
		protected string branch_manage;
		protected DateTime creation_date;
		
		public SBTableBranches() : base("branches", "branch_id")
		{
		}
	}
}

