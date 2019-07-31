using System;
using System.IO;
using Nini.Config;

namespace SinticBolivia
{
	public class SBConfig
	{
		protected static string cfg_file;
		protected static XmlConfigSource cfg;
		
		public SBConfig ()
		{
		}
		/// <summary>
		/// Initialize config file
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>
		/// </param>
		public static void setCfgFile(string file)
		{
			SBConfig.cfg_file = file;
			if( !File.Exists(file) )
			{
				FileStream stream = File.Create(file);
				byte[] text = System.Text.Encoding.ASCII.GetBytes("<Nini></Nini>");
				stream.Write(text, 0, text.Length);
				stream.Close();
			}
				
			SBConfig.cfg = new XmlConfigSource(SBConfig.cfg_file);
			cfg.AutoSave = true;
		}
		public static void update(string name, object cfg_value)
		{
			
			if(SBConfig.cfg.Configs["app_cfg"] == null )
			{
				SBConfig.cfg.Configs.Add("app_cfg");
			}
			SBConfig.cfg.Configs["app_cfg"].Set(name, cfg_value);
		}
		public static object getValue(string key, string default_val)
		{
			if( SBConfig.cfg.Configs["app_cfg"] == null )
				return default_val;
			return SBConfig.cfg.Configs["app_cfg"].Get(key, default_val);
		}
	}
}

