using System;

namespace SinticBolivia.Database
{
	public abstract class Entity : SBObject
	{
		protected 	string 		table;
		protected	SBDatabase	dbh;
		
		public Entity()
		{
			
		}
		public T Get<T>(long id)
		{
			var obj = default(T);
			
			return obj;
		}
	}
}

