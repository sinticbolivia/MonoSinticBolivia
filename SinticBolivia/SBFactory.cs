using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using SinticBolivia.Database;
using SinticBolivia.Web;

namespace SinticBolivia
{
	public class SBFactory
	{
		/// <summary>
		/// Database configuration
		/// Hashtable ["db_type"] = sql_server | mysql | sqlite | postgres
		/// Hashtable ["con_string"] = "Server=localhost;Database=test;User ID=myuserid;Password=mypassword;Pooling=false";
		/// </summary>
        public static Hashtable dbh_config;
        private static SBDatabase dbh = null;
		private static SBWebApplication app;
        private static SBUser user;
		private static SBIApplication desktop_app;
        public static string FactoryType;
        public HttpContext Context;

		public SBFactory ()
		{
		}
		public static SBDatabase getDbh(bool new_connection = false)
        {
			if( SBFactory.dbh_config == null )
			{
				throw new Exception("Database connection data is not assigned");
			}
            SBDatabase _dbh = null;
            if (SBFactory.FactoryType == "web")
            {
                if(HttpContext.Current.Items["dbh"] != null )
                    return (SBDatabase)HttpContext.Current.Items["dbh"];
                Console.WriteLine("Creation database instance");
                //create a new instance of database handler
                if( SBFactory.dbh_config["db_type"].ToString().Equals("sql_server"))
                {
                    _dbh = new SqlServer(SBFactory.dbh_config["con_string"].ToString());
                    _dbh.open();
                }
                if( SBFactory.dbh_config["db_type"].ToString().Equals("mysql"))
                {
                    _dbh = new MySQL(SBFactory.dbh_config["con_string"].ToString());
                    if( !_dbh.open() )
                        throw new Exception("Enable to connect MySql Database");
                }
                if( SBFactory.dbh_config["db_type"].ToString().Equals("sqlite") )
                {
                    _dbh = new SQLite(SBFactory.dbh_config["con_string"].ToString());
                    if( !_dbh.open() )
                        throw new Exception("Unable to connect to SQLite Database");
                }
                HttpContext.Current.Items["dbh"] = _dbh;
                return _dbh;
            } 
            else
            {
                if( SBFactory.dbh == null || new_connection)
                {
                    Console.WriteLine("Creation database instance");
                    //create a new instance of database handler
                    if( SBFactory.dbh_config["db_type"].ToString().Equals("sql_server"))
                    {
                        _dbh = new SqlServer(SBFactory.dbh_config["con_string"].ToString());
                        _dbh.open();
                    }
                    if( SBFactory.dbh_config["db_type"].ToString().Equals("mysql"))
                    {
                        _dbh = new MySQL(SBFactory.dbh_config["con_string"].ToString());
                        if( !_dbh.open() )
                            throw new Exception("Enable to connect MySql Database");
                    }
                    if( SBFactory.dbh_config["db_type"].ToString().Equals("sqlite") )
                    {
                        _dbh = new SQLite(SBFactory.dbh_config["con_string"].ToString());
                        if( !_dbh.open() )
                            throw new Exception("Unable to connect to SQLite Database");
                    }
                    if( new_connection )
                        return _dbh;
                    SBFactory.dbh = _dbh;
                }
            }

            return SBFactory.dbh;
        }
        public static void DestroyDbh()
        {
            SBFactory.dbh = null;
        }
		public static SBIApplication getApp()
		{
			if( SBFactory.desktop_app == null )				
			{
				SBFactory.desktop_app = (SBIApplication)new SBApplication();
			}
			return (SBIApplication)SBFactory.desktop_app;
		}
		/// <summary>
		/// GET wen application instance 
		/// </summary>
		/// <returns>
		/// A <see cref="SMWebApplication"/>
		/// </returns>
		public static SBWebApplication getWebApp()
		{
			if( SBFactory.app == null )				
			{
				SBFactory.app = new SBWebApplication();
			}
			return SBFactory.app;
		}
        public static SBUser getWebUser()
        {
            //verificar si el usuario existe en sesion
            if(SBSession.getVar("user") == null)
            {
                SBFactory.getWebApp().logString("usuario en sesion no existe");
                SBFactory.user = new SBUser();
            }
            else
            {

                SBFactory.user = (SBUser)SBSession.getVar("user");
                SBFactory.getWebApp().logString("usuario en sesion existe " + SBFactory.user.Username);
            }
            return SBFactory.user;
        }
	}
}

