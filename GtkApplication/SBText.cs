using System;
using Mono.Unix;

namespace SinticBolivia
{
	public class SBText
	{
		public SBText ()
		{
		}
		public static void loadLanguage(string package, string locale_dir)
		{
			Catalog.Init(package, locale_dir);
		}
		/// <summary>
		/// Translate a string using gettext
		/// </summary>
		/// <param name='string_id'>
		/// String_id.
		/// </param>
		public static string _(string string_id)
		{
			string translated_str = Catalog.GetString(string_id);
			return SBModule.do_action("lang_string", translated_str).ToString();	
			/*
			object res = SBModule.do_action("lang_string", translated_str);
			if( res == null )
				return translated_str;
			return res.ToString();
			*/
		}
		/// <summary>
		/// Translate a string using gettext and a specific domain
		/// </summary>
		/// <param name='string_id'>
		/// String_id.
		/// </param>
		/// <param name='domain'>
		/// Domain.
		/// </param>
		public static string _(string string_id, string domain)
		{
			string translated_str = Catalog.GetString(string_id);
			return SBModule.do_action("lang_string", translated_str).ToString();
		}
	}
}

