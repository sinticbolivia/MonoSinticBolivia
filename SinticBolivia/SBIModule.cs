using System;

namespace SinticBolivia
{
	public interface SBIModule
	{
		string Name{get;}
		string Description{get;}
		string Author{get;}
		string Version{get;}
        string Key{get;}
		
        void OnEnable();
        void OnDisable();
		void loadModule();
		void unloadModule();
		void InitModule();
	}
}

