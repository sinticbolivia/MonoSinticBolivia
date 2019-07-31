using System;
using System.Collections;
using System.Reflection;
using System.IO;

namespace SinticBolivia
{
	// ---------------------------------------------
	// now do it the efficient way
	// by holding references to the assembly
	// and class

	// this is an inner class which holds the class instance info
	public class DynaClassInfo
	{
		public Type type;
		public Object ClassObject;

		public DynaClassInfo ()
		{
		}

		public DynaClassInfo (Type t, Object c)
		{
			type = t;
			ClassObject = c;
		}
	}
	/// <summary>
	/// Event Handler for module hooks
	/// </summary>
	public delegate object ActionHandler(params object[] list);	
	
	public class SBModule
	{
		public static Hashtable AssemblyReferences = new Hashtable ();
		public static Hashtable ClassReferences = new Hashtable ();
		protected static Hashtable available_modules;
        public static Hashtable assemblies;
		protected static string name_space_modules;
		protected static Hashtable actions;
		
		public SBModule ()
		{
		}
		// this way of invoking a function
		// is slower when making multiple calls
		// because the assembly is being instantiated each time.
		// But this code is clearer as to what is going on
		public static object InvokeMethodSlow (string AssemblyName, string ClassName, string MethodName, Object[] args)
		{
			// load the assemly
			Assembly assembly = Assembly.LoadFrom (AssemblyName);
			
			// Walk through each type in the assembly looking for our class
			foreach (Type type in assembly.GetTypes ()) 
			{
				if (type.IsClass == true) 
				{
					if (type.FullName.EndsWith ("." + ClassName)) 
					{
						// create an instance of the object
						object ClassObj = Activator.CreateInstance (type);
						// Dynamically Invoke the method
						object Result = type.InvokeMember (MethodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, ClassObj, args);
						return (Result);
					}
				}
			}
			throw (new System.Exception ("could not invoke method"));
		}
		public static DynaClassInfo GetClassReference (string AssemblyName, string ClassName)
		{
			if (ClassReferences.ContainsKey (AssemblyName) == false) 
			{
				Assembly assembly;
				if (AssemblyReferences.ContainsKey (AssemblyName) == false) 
				{
					AssemblyReferences.Add (AssemblyName, assembly = Assembly.LoadFrom (AssemblyName));
				} 
				else
					assembly = (Assembly)AssemblyReferences[AssemblyName];
				
				// Walk through each type in the assembly
				foreach (Type type in assembly.GetTypes ()) 
				{
					if (type.IsClass == true) 
					{
						// doing it this way means that you don't have
						// to specify the full namespace and class (just the class)
						if (type.FullName.EndsWith ("." + ClassName)) 
						{
							DynaClassInfo ci = new DynaClassInfo (type, Activator.CreateInstance (type));
							ClassReferences.Add (AssemblyName, ci);
							return (ci);
						}
					}
				}
				throw (new System.Exception ("could not instantiate class"));
			}
			return ((DynaClassInfo)ClassReferences[AssemblyName]);
		}
		public static object InvokeMethod (DynaClassInfo ci, string MethodName, Object[] args)
		{
			// Dynamically Invoke the method
			Object Result = ci.type.InvokeMember (MethodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, ci.ClassObject, args);
			return (Result);
		}

		// --- this is the method that you invoke ------------
		public static object InvokeMethod (string AssemblyName, string ClassName, string MethodName, Object[] args)
		{
			DynaClassInfo ci = GetClassReference (AssemblyName, ClassName);
			return (InvokeMethod (ci, MethodName, args));
		}
		/// <summary>
		/// Set the namespace for modules
		/// </summary>
		public static void setNameSpaceModules(string ns)
		{
			SBModule.name_space_modules = ns;
		}
        /// <summary>
        /// Loads the modules.
        /// </summary>
        /// <returns>The modules.</returns>
        /// <param name="path">Path where modules assemblies are installed.</param>
        /// <param name="prefix">Module main class Prefix.</param>
		public static string[] loadModules(string path, string prefix)
		{
			SBModule.available_modules = new Hashtable();
            SBModule.assemblies = new Hashtable();
			if( !Directory.Exists(path) )
				throw new Exception("Modules folder does not exists");

			//get available modules
			string[] modules = Directory.GetFiles(path, prefix + "*.dll");
			
			foreach(string module in modules)
			{
				try
				{
                    //Console.WriteLine("Module => {0}", module);
					string[] stack = module.Split(Path.DirectorySeparatorChar);
					string module_name = stack[stack.Length -1];
					module_name = module_name.Substring(0, module_name.Length - 4);
					//check prefix
					
					//Assembly ass = Assembly.Load(module_name);	
					Assembly ass = Assembly.LoadFrom(module);	
                    //AppDomain.CurrentDomain.Load(ass.GetName());

					if( ass != null )
					{
                        string module_class = String.Format("{0}.{1}", SBModule.name_space_modules, module_name);
                        Console.WriteLine("Module loaded: " + module_name);
                        Console.WriteLine("Module class: " + module_class);
						Type ObjType = ass.GetType(module_class);
						//Type ObjType = ass.GetType(String.Format("{0}.{1}", SMModule.name_space_modules, "SMIModule"));
						if( ObjType != null )
						{
							//Console.WriteLine("Module Type: " + ObjType.ToString());
                            SBIModule the_module = (SBIModule)Activator.CreateInstance(ObjType);
							SBModule.available_modules.Add(the_module.Key, the_module);	
                            SBModule.assemblies.Add(the_module.Key, ass);  
						}
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine("Error loading module");
					Console.WriteLine(ex.Message);	
				}
			}	
			
			return modules;
		}
		public static void attachModule(string name, SBIModule module)
		{
			SBModule.available_modules.Add(name, module);
			module.loadModule();
		}
		public static bool moduleExists(string module)
		{
			return SBModule.available_modules.ContainsKey(module);
		}
		public static SBIModule getModule(string module)
		{
			if( !SBModule.moduleExists(module) )
				return null;
			return (SBIModule)SBModule.available_modules[module];
		}
		public static Hashtable getModules()
		{
			return SBModule.available_modules;
		}
        /// <summary>
        /// Gets the installed modules into modules folder.
        /// </summary>
        /// <returns>The installed modules.</returns>
        public static ArrayList GetInstalledModules(string path, string prefix = "Mod")
        {
            //get available modules
            string[] modules = Directory.GetFiles(path, prefix + "*.dll");
           
            return new ArrayList(modules);
        }
		/// <summary>
		/// Invoke a module method
		/// </summary>
		/// <param name="module_name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="method">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="list">
		/// A <see cref="System.Object[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Object"/>
		/// </returns>
		public static object callMethod(string module_name, string method, params object[] list)
		{
			SBIModule mod = SBModule.getModule(module_name);
			Type mod_type = mod.GetType();
			return mod_type.GetMethod(method).Invoke(mod, list);
		}
		public static void add_action(string action, ActionHandler handler)
		{
			if( SBModule.actions == null )
			{
				SBModule.actions = new Hashtable();
			}
			
			if( SBModule.actions.ContainsKey(action) )
			{
				((ArrayList)SBModule.actions[action]).Add(handler);
			}
			else
			{
				
				SBModule.actions.Add(action, new ArrayList());	
				((ArrayList)SBModule.actions[action]).Add(handler);
			}
			
		}
		public static object do_action(string action, params object[] args)
		{
			if( SBModule.actions == null )
			{
				if( args.Length > 0 )
					return args[0];

				return null;
			}
			if( !SBModule.actions.ContainsKey(action) )
			{
				if( args.Length > 0 )
					return args[0];
				
				return null;
			}
			object res = "";
			foreach(ActionHandler handler in ((ArrayList)SBModule.actions[action]))
			{
				//Console.WriteLine(handler.ToString());
				res = handler(args);
			}
			return res;
		}
        public static bool IsModuleEnabled(string module_key)
        {
            string query = string.Format("SELECT module_id FROM modules WHERE module_key = '{0}' AND status = 'enabled'", module_key);
            if (SBFactory.getDbh().QueryRow(query) != null)
                return true;
            return false;
        }
	}
}

