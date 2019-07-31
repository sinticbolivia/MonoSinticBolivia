using System;

namespace SinticBolivia.Database
{
	public class EntityAttribute : Attribute
	{
		public bool PrimaryKey 		= false;
		public bool AutoIncrement	= false;
		public bool Nullable		= true;
		public bool Blank 			= true;
		
		public EntityAttribute()
		{
		}
	}
}

