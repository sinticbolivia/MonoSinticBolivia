using System;
namespace SinticBolivia
{
	public interface SBIGtkModule : SBIModule
	{
		SBIGtkApplication App{get;set;}
		void Config();
	}
}

