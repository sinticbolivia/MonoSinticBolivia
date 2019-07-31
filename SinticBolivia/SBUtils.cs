using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;

namespace SinticBolivia
{
	public class SBUtils
	{
		public SBUtils ()
		{
		}
		public static string var_dump(object obj, int recursion)
		{
		    StringBuilder result = new StringBuilder();
		  	
		    // Protect the method against endless recursion
		    if (recursion < 5)
		    {
		        // Determine object type
		        Type t = obj.GetType();
		  
		        // Get array with properties for this object
		        FieldInfo[] properties = t.GetFields();
		  		
		        foreach (FieldInfo property in properties)
		        {
					
		            try
		            {
		                // Get the property value
		                object value = property.GetValue(obj);
		  
		                // Create indenting string to put in front of properties of a deeper level
		                // We'll need this when we display the property name and value
		                string indent = String.Empty;
		                string spaces = "|   ";
		                string trail = "|...";
		                  
		                if (recursion > 0)
		                {
		                    indent = new StringBuilder(trail).Insert(0, spaces, recursion - 1).ToString();
		                }
		  
		                if (value != null)
		                {
		                    // If the value is a string, add quotation marks
		                    string displayValue = value.ToString();
		                    if (value is string) displayValue = String.Concat('"', displayValue, '"');
		  
		                    // Add property name and value to return string
		                    result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, displayValue);
		  
		                    try
		                    {
		                        if (!(value is ICollection))
		                        {
		                            // Call var_dump() again to list child properties
		                            // This throws an exception if the current property value
		                            // is of an unsupported type (eg. it has not properties)
		                            result.Append(var_dump(value, recursion + 1));
		                        }
		                        else
		                        {
		                            // 2009-07-29: added support for collections
		                            // The value is a collection (eg. it's an arraylist or generic list)
		                            // so loop through its elements and dump their properties
		                            int elementCount = 0;
		                            foreach (object element in ((ICollection)value))
		                            {
		                                string elementName = String.Format("{0}[{1}]", property.Name, elementCount);
		                                indent = new StringBuilder(trail).Insert(0, spaces, recursion).ToString();
		  
		                                // Display the collection element name and type
		                                result.AppendFormat("{0}{1} = {2}\n", indent, elementName, element.ToString());
		  
		                                // Display the child properties
		                                result.Append(var_dump(element, recursion + 2));
		                                elementCount++;
		                            }
		  
		                            result.Append(var_dump(value, recursion + 1));
		                        }
		                    }
		                    catch(Exception e) { Console.WriteLine(e.Message);}
		                }
		                else
		                {
		                    // Add empty (null) property to return string
		                    result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, "null");
		                }
		            }
		            catch(Exception e)
		            {
		                // Some properties will throw an exception on property.GetValue()
		                // I don't know exactly why this happens, so for now i will ignore them...
						Console.WriteLine(e.Message);
		            }
		        }
		    }
		  
		    return result.ToString();
		}
        /// <summary>
        /// Returns the first day of the week that the specified
        /// date is in using the current culture.
        /// </summary>
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return SBUtils.GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
        }
         
        /// <summary>
        /// Returns the first day of the week that the specified date
        /// is in.
        /// </summary>
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);
         
            return firstDayInWeek;
        }
        public static void log(string file, object text)
        {
            StreamWriter log;

            if (!File.Exists(file))
            {
                log = new StreamWriter(file);
            }
            else
            {
                log = File.AppendText(file);
            }

            // Write to the file:
            log.WriteLine(SBUtils.var_dump(text, 0));
            log.WriteLine();

            // Close the stream:
            log.Close();
        }
		public static void logString(string file, string text)
        {
            StreamWriter log;

            if (!File.Exists(file))
            {
                log = new StreamWriter(file);
            }
            else
            {
                log = File.AppendText(file);
            }

            // Write to the file:
            log.WriteLine(string.Format("[{0}]#\n{1}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text));
            // Close the stream:
            log.Close();
        }
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="cmd">Cmd.</param>
        public static void ExecuteCommand(string cmd, string args, bool console = false)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(cmd, args);
            psi.UseShellExecute = console;
            System.Diagnostics.Process.Start(psi);

        }
        public static string FillLeftCeros(int number, int max_length)
        {
            string ss = "";
            for(int i = 0; i < (max_length - (number.ToString().Length)); i++)
            {
                ss += "0";
            }
            ss += number.ToString();
            
            return ss;
        }
	}
}

