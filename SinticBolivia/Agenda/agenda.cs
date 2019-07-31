using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using SmartBiz;
using SmartBiz.Database;

namespace SmartBiz.Agenda
{
    public class SMAgenda : SMObject
    {
        public int id;
        public int id_objeto;
		public int id_tipo_objeto;
        public string descripcion;
        public DateTime fecha_inicio;
        public DateTime fecha_termino;
        public DateTime fecha_creacion;
		
		protected SMObjetoAgenda obj;
		
        public SMAgenda()
        {
 
        }
		public SMAgenda(int id)
		{
			this.obtenerInformacionPorID(id);
		}
        public int agendarObjeto(SMObjetoAgenda obj)
        {
			this.id = Int32.Parse(SMFactory.getDbh().getTableCode("agenda", "id").ToString());
			//obtener un id para la nueva instance del objeto a agendar
			this.id_objeto = Int32.Parse(SMFactory.getDbh().getTableCode("agenda", "id_objeto").ToString());
			//asignar el tipo de objeto
			this.id_tipo_objeto = obj.id_tipo_objeto;
			this.fecha_creacion = DateTime.Now;
			int id_entrada = SMFactory.getDbh().insert("agenda", this);
			//almacenar las propiedades del objeto
			ArrayList propiedades = obj.obtenerPropiedades();
			if( propiedades != null )
			{
				foreach(Hashtable key in propiedades)
				{
                    Hashtable p = new Hashtable();
                    p.Add("id_propiedad", SMFactory.getDbh().getTableCode("agenda_objeto_propiedades", "id_propiedad"));
                    p.Add("id_objeto", this.id_objeto);
                    p.Add("propiedad", key["propiedad"]);

                    p.Add("valor", key["valor"]);
                    p.Add("fecha_creacion", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                    SMFactory.getDbh().insert("agenda_objeto_propiedades", p);
					
				}
			}
			
			return id_entrada;
        }
        public bool obtenerInformacionPorID(int id)
        {
            string query = "SELECT " + this.getPropertiesWithGlue(",") +
                            " FROM agenda " +
                            "WHERE id = " + id.ToString();
            Hashtable row = SMFactory.getDbh().QueryRow(query);
            if (row == null)
                throw new Exception("Objecto de agenda no encontrado");
			/*
            this.id_objeto = Int32.Parse(row["id_objeto"].ToString());
            this.descripcion = row["descripcion"].ToString();
            this.fecha_inicio = DateTime.Parse(row["fecha_inicio"].ToString());
            this.fecha_termino = DateTime.Parse(row["fecha_termino"].ToString());
            */
			this.bind(row);
			//obtener el objeto asociado a la agenda
			//this.obj = new SMObjetoAgenda(this.id_objeto);
			
            return true;
        }
		public static SMAgenda obtenerEntradaPorIDObjecto(int id_objeto)
		{
			SMAgenda entrada = new SMAgenda();
			string query = "SELECT " + entrada.getPropertiesWithGlue(",") +
                            " FROM agenda " +
                            "WHERE id_objeto = " + id_objeto.ToString();
			Hashtable row = SMFactory.getDbh().QueryRow(query);
            if (row == null)
                throw new Exception("Objecto de agenda no encontrado");
			
			entrada.bind(row);
			return entrada;
		}
		/// <summary>
		/// Crear una nueva instance de un objeto de agenda
		/// </summary>
		/// <param name="tipo_objeto">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="SMObjetoAgenda"/>
		/// </returns>
		public SMObjetoAgenda crearObjeto(string tipo_objeto)
        {
            SMObjetoAgenda obj = new SMObjetoAgenda();
            string query = "SELECT id_tipo_objeto, tipo_objeto, fecha_creacion " +
            				"FROM agenda_tipo_objetos "+
                            " WHERE tipo_objeto = '"+tipo_objeto+"'";
            Hashtable row = SMFactory.getDbh().QueryRow(query);
			if( row != null )
    	        obj.bind(row);
            return obj;
        }
		/// <summary>
		/// Obtiene el identificador de un tipo de objeto determinado 
		/// </summary>
		/// <param name="tipo_objeto">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public static int obtenerTipoObjetoID(string tipo_objeto)
		{
			string query = "SELECT id_tipo_objeto, tipo_objeto, fecha_creacion " +
							"FROM agenda_tipo_objetos "+
                            " WHERE tipo_objeto = '"+tipo_objeto+"'";
            Hashtable row = SMFactory.getDbh().QueryRow(query);
			if( row == null )
    	        return 0;
            return Int32.Parse(row["id_tipo_objeto"].ToString());
		}
		/// <summary>
		/// Obtiene el objeto asociado a la entrada de la agenda 
		/// </summary>
		/// <returns>
		/// A <see cref="SMObjetoAgenda"/>
		/// </returns>
		public SMObjetoAgenda obtenerObjeto()
		{
			return this.obj;
		}
		public void asignarObjeto(SMObjetoAgenda o)
		{
			this.obj = o;
		}
    }
}
