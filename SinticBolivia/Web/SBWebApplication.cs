using System;
using System.Web;
using System.Collections;

namespace SinticBolivia.Web
{
    public class SBWebApplication : SBApplication
	{
        public SBWebApplication(string log_file = "app.log") : base(log_file)
		{
            SBApplication.BASE_PATH = HttpContext.Current.Server.MapPath("~");
            //set log file
            this.LogFile = BASE_PATH + DS + log_file;
		}
        ~SBWebApplication ()
        {
            Console.WriteLine("Destroying SBWebApplication");
        }
		public void initSession()
		{
			if( SBRequest.context.Session["messages"] == null )
			{
				SBRequest.context.Session["messages"] = new ArrayList();
			}
		}
		public void setContext(HttpContext con, HttpRequest req)
        {
            SBRequest.setContext(con);
            SBRequest.SetRequest(req);
        }
		/// <summary>
		/// Adiciona un nuevo mensaje a la pila de la aplicacion
		/// </summary>
		/// <param name="msg">
		/// A <see cref="System.String"/>
		/// </param>
		/// /// <param name="tipo">
		/// A <see cref="System.String"/>
		/// </param>
		public void adicionarMensaje(string msg, string tipo)
        {
            if (SBRequest.context.Session ["messages"] == null)
            {
                SBFactory.getWebApp().initSession();
            }
            ((ArrayList)SBRequest.context.Session["messages"]).Add(string.Format("<div class=\"alert alert-{0}\">{1}</div>", tipo, msg));
		}
		/// <summary>
		/// Obtiene los mensajes en la pila de la aplicacion dentro de una lista
		/// </summary>
		/// <returns>
		/// A <see cref="ArrayList"/>
		/// </returns>
		public ArrayList obtenerMensajes()
		{
			return (ArrayList)SBRequest.context.Session["messages"];
		}
		/// <summary>
		/// Obtiene los mensajes en la pila de la aplicacion dentro de una cadena de texto
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public string obtenerMensajesStr()
		{
			string str = "";
			foreach(string msg in (ArrayList)SBRequest.context.Session["messages"])
			{
				str += msg.ToString();	
			}
			((ArrayList)SBRequest.context.Session["messages"]).Clear();
			return str;
		}
		/// <summary>
		/// Imprime todos los mensajes que estan dentro la pila de la aplicacion
		/// </summary>
		public void imprimirMensajes()
		{
			foreach(string msg in ((ArrayList)SBRequest.context.Session["messages"]))
			{
				SBRequest.context.Response.Write("<div class=\"\">"+msg+"</div>");	
			}
			((ArrayList)SBRequest.context.Session["messages"]).Clear();
		}
		public void actualizarParametro(string nombre, string valor)
		{
			Hashtable data = new Hashtable();
			string query = "SELECT id_parametro FROM parametros WHERE nombre_parametro = '"+nombre+"'";
			if( SBFactory.getDbh().QueryRow(query) == null )
			{
				//crear parametros
				data.Add("nombre_parametro", nombre);
				data.Add("valor_parametro", valor);
				data.Add("fecha_creacion", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
				SBFactory.getDbh().insert("parametros", data);
			}
			else
			{
				//actualizar parametro
				data.Add("valor_parametro", valor);
				Hashtable w = new Hashtable();
				w.Add("nombre_parametro", nombre);
				SBFactory.getDbh().update("parametros", data, w);
			}
		}
		public object obtenerParametro(string nombre, object default_val)
		{
			string query = "SELECT id_parametro, nombre_parametro, valor_parametro FROM parametros WHERE nombre_parametro = '"+nombre+"'";
			this.logString(query);
			Hashtable p = SBFactory.getDbh().QueryRow(query);
			if( p == null )
			{
				//crear parametros
				return default_val;
			}
			return p["valor_parametro"];
		}
		public void setLogFile(string file)
		{
			this.LogFile = SBRequest.context.Request.PhysicalApplicationPath + SBApplication.DS + file ;
		}
		public void logString(string str, bool console = true)
		{
			SBUtils.logString(this.LogFile, str);
            if (console)
                Console.WriteLine(str);
		}
		public void log(object obj)
		{
			SBUtils.log(this.LogFile, obj);
		}

	}
}

