using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SinticBolivia.Web
{
    public class SBWebUtils
    {
        public static string GetAppRootUrl(bool endSlash)
        {
            string host = SBRequest.context.Request.Url.GetLeftPart(UriPartial.Authority);

            string appRootUrl = SBRequest.context.Request.ApplicationPath;
            if (!appRootUrl.EndsWith("/")) //a virtual
            {
                appRootUrl += "/";
            }
            if (!endSlash)
            {
                appRootUrl = appRootUrl.Substring(0, appRootUrl.Length - 1);
            }
            return host + appRootUrl;
        }

    }
}
