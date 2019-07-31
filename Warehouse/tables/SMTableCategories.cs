using System;
using SinticBolivia.Database;

namespace SinticBolivia.Warehouse.Tables
{
	public class SBTableProductCategories : SBTable
	{
		protected int category_id;
		protected string name;
		protected string description;
		protected int parent;
		protected DateTime creation_date;
		
		public SBTableProductCategories () : base("categories", "category_id")
		{
		}
	}
}

