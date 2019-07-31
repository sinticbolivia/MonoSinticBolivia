using System;

namespace SinticBolivia
{
	public interface SBIApplication
	{
		void loadModules();
		void initializeModules();
		void updateParameter(string name, object vvalue);
		object getParameter(string p_name, object default_val);
		void setLogFile(string file);
		void logString(string str, bool write_console);
		void log(object obj);
	}
}

