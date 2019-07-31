using System;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using SinticBolivia.Database.Tables;

namespace SinticBolivia
{
	public class SBApplication
	{
        public static string DS;
        public static string OS;
        public static string BASE_PATH;
        public static string TEMP_PATH;
		protected string LogFile = "";

		public SBApplication (string log_file = "mb.log")
		{
            ///check OS
            OperatingSystem os = Environment.OSVersion;
            SBApplication.OS = os.Platform.ToString().Trim();
            SBApplication.DS = System.IO.Path.DirectorySeparatorChar.ToString();
            SBApplication.BASE_PATH = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            SBApplication.TEMP_PATH = BASE_PATH + DS + "temp";
            if (!System.IO.Directory.Exists(TEMP_PATH))
                System.IO.Directory.CreateDirectory(TEMP_PATH);
            //set log file
            this.LogFile = BASE_PATH + DS + log_file;
		}
		public virtual void loadModules(){}
		public virtual void initializeModules(){}
		public void updateParameter(string name, object vvalue)
		{
			SBTableParameters table = new SBTableParameters();
			Hashtable param = table.getRow(string.Format("key = '{0}'", name));
			if(param == null)
			{
				this.addParameter(name, vvalue);
			}
			else
			{
				param["value"] = vvalue;
				table.updateRow(param);
			}
		}
		public object getParameter(string p_name, object default_val)
		{
			SBTableParameters table = new SBTableParameters();
			Hashtable param = table.getRow(string.Format("key = '{0}'", p_name));
			if( param == null )
			{
				return default_val;
			}
			return param["value"];
		}
		public bool addParameter(string p_name, object p_value)
		{
			SBTableParameters table = new SBTableParameters();
			Hashtable param = new Hashtable();
			param.Add("key", p_name);
			param.Add("value", p_value);
			param.Add("creation_date", DateTime.Now);
			table.insertRow(param);
			return true;
		}
		public void setLogFile(string file)
		{
			this.LogFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" +file ;
		}
		public void logString(string str, bool write_console = true)
		{
            //str = string.Format("[{0}]#\n{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), str);
			SBUtils.logString(this.LogFile, str);
            if (write_console)
                Console.WriteLine(string.Format("[{0}]#\n{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), str));
		}
		public void log(object obj)
		{
			SBUtils.log(this.LogFile, obj);
		}
	}
}

