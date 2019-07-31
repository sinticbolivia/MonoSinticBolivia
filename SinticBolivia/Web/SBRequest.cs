using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.SessionState;

namespace SinticBolivia.Web
{
    public class SBRequest
    {
        public static HttpContext context;
        private static HttpRequest request;

        private SBRequest()
        { 
        }
        public static void setContext(HttpContext con)
        {

            SBRequest.context = con;
            //HttpContext.Current.re
            SBRequest.request = SBRequest.context.Request;
        }
        public static void SetRequest(HttpRequest req)
        {
            SBRequest.request = req;
        }
        public static object getVar(string parameter)
        {
            //if (SBRequest.request.HttpMethod.ToUpper() == "GET")
            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "GET")
            {
                //if (SBRequest.request.QueryString [parameter] != null)
                if(HttpContext.Current.Request.QueryString[parameter] != null)
                {
                    return HttpContext.Current.Request.QueryString [parameter];
                } else
                {
                    return null;
                }
            } 
            else
            {
                if( HttpContext.Current.Request.Form[parameter] != null )
                    return HttpContext.Current.Request.Form[parameter];
                else if (HttpContext.Current.Request[parameter] != null)
                    return HttpContext.Current.Request[parameter];
                else if (HttpContext.Current.Request.QueryString[parameter] != null)
                    return HttpContext.Current.Request.QueryString[parameter];
            }

            return null;
        }
        public static string getTask()
        {
            object task = SBRequest.getVar("task");
            if (task != null)
                return task.ToString();
            return "";
        }
		public static string getString(string parameter)
		{
			object val = SBRequest.getVar(parameter);
			if( val == null )
				return "";
			return val.ToString().Trim();
		}
		public static int getInt32(string parameter)
		{
			object val = SBRequest.getVar(parameter);
			if( val == null )
				return 0;
            if( string.IsNullOrEmpty(val.ToString()) )
                return 0;
			return Int32.Parse(val.ToString());
		}
    }
}
