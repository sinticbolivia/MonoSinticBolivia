using System;
using SinticBolivia;

namespace SinticBolivia.Web
{
	public class SBRoute
	{
		public SBRoute ()
		{
		}
		public static string link(string page)
		{
			string link = SBWebUtils.GetAppRootUrl(true) + page;
			return link;
		}
		public static void redirect(string page)
		{
			SBRequest.context.Response.Redirect(SBRoute.link(page));
			SBRequest.context.Response.End();
		}
	}
}

