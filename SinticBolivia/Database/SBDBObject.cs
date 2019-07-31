

namespace SinticBolivia.Database
{
	public abstract class SBDBObject : SBObject
	{
		
		//public IDataReader reader;
		public abstract bool open();
		public abstract bool close();
		//public abstract IDataReader QueryDatareader(string query);
		
		
        
	}
}

