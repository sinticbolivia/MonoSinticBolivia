using System;
using System.Collections;

namespace SinticBolivia
{
	public class SBGlobals
	{
		protected static Hashtable globals;
		
		public SBGlobals ()
		{

		}
		protected static void init()
		{
			if( SBGlobals.globals == null )
			{
				SBGlobals.globals = new Hashtable();
			}
		}
		/// <summary>
		/// Set a global var
		/// 
		/// </summary>
		/// <returns>The variable.</returns>
		/// <param name="var_name">Variable name.</param>
		public static object getVar(string var_name)
		{
			SBGlobals.init();
			if( !SBGlobals.globals.ContainsKey(var_name) )
				return null;
			
			return SBGlobals.globals[var_name];
		}
		public static void setVar(string var_name, object var_value)
		{
			SBGlobals.init();
			if (SBGlobals.globals.ContainsKey(var_name))
			{
				SBGlobals.globals[var_name] = var_value;
				return;
			}
			SBGlobals.globals.Add(var_name, var_value);
		}
	}
}

