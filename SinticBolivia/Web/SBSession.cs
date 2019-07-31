using System;
using SinticBolivia.Database;

namespace SinticBolivia.Web
{
	public class SBSession
	{
		public SBSession ()
		{
		}
		public static object getVar (string varname)
		{
			if (SBRequest.context.Session[varname] == null)
				return null;
			return SBRequest.context.Session[varname];
		}
		public static void setVar (string varname, object var_value)
		{
			SBRequest.context.Session[varname] = var_value;
		}
	}
}

