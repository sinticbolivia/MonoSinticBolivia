using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Data;
using SmartBiz.Database;
using System.Web.Script.Serialization;

namespace SmartBiz.Agenda
{
    public class SMObjetoAgenda : SMObject
    {
        private int id_objeto;

        public int id_tipo_objeto;
        public string tipo_objeto;
        public DateTime fecha_creacion;
		
		private ArrayList propiedades;
		
        public SMObjetoAgenda()
        { 

        }
		public SMObjetoAgenda(int id_objeto)
        { 
			this.obtenerInformacion(id_objeto);
        }
        public bool obtenerInformacion(int id_objeto)
        {
			this.propiedades = new ArrayList();
			
            string query = "SELECT a.id_objeto, a.id_tipo_objeto, o.tipo_objeto " + 
                            " FROM agenda a, agenda_tipo_objetos o "+
                            "WHERE a.id_objeto = " + id_objeto.ToString() + " " +
                            "AND a.id_tipo_objeto = o.id_tipo_objeto ";
            Hashtable row = SMFactory.getDbh().QueryRow(query);
            if (row == null)
                throw new Exception("Objecto de agenda no encontrado");
			
            this.bind(row);
			query = "SELECT id_propiedad, id_objeto, propiedad, valor, fecha_creacion " +
						"FROM agenda_objeto_propiedades " +
						"WHERE id_objeto = " + id_objeto.ToString();
			this.propiedades = SMFactory.getDbh().Query(query);
            /*
			foreach(Hashtable r in list)
			{
				if(this.propiedades.ContainsKey(r["propiedad"].ToString()))
				{
					this.propiedades.Add(r["propiedad"].ToString() + "_" + r["valor"].ToString(), r["valor"].ToString());
				}
				else
					this.propiedades.Add(r["propiedad"].ToString(), r["valor"].ToString());
			}
            */         
            this.id_objeto = id_objeto;
            return true;
        }
		public ArrayList obtenerPropiedades()
		{
			return this.propiedades;
		}
        public void asignarPropiedades(ArrayList propiedades)
		{
			this.propiedades = propiedades;
		}
        public bool actualizar()
        {
            string query = "UPDATE agenda SET ";
            foreach(FieldInfo column in this.getProperties())
            {
                if (column.GetValue(this) is string )
                {
                    query += column.Name + " = '" + column.GetValue(this) + "',";
                }
                else if(column.GetValue(this) is int)
                {
                    query += column.Name + " = '" + column.GetValue(this) + "',";
                }
            }
            query = query.Substring(query.Length, query.Length - 1);
            query += " WHERE id = " + this.id_tipo_objeto;
            SMFactory.getDbh().Execute(query);
            return true;
        }
        public bool eliminarObjeto(int id)
        {
            string query = "DELETE FROM agenda WHERE id = " + id;
            SMFactory.getDbh().Execute(query);
            return true;
        }
        /// <summary>
        /// Adiciona una nueva propiedad en la base de datos para un objeto de agenda
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="valor"></param>
        public int adicionarPropiedad(string nombre, string valor)
        {
            string query = "INSERT INTO agenda_objeto_propiedades(id_propiedad, id_objeto, propiedad, valor, fecha_creacion) "+
                            "VALUES({0}, {1}, '{2}', '{3}', '{4}')";
            query = String.Format(query, 
                                    SMFactory.getDbh().getTableCode("agenda_objeto_propiedades", "id_propiedad"),
                                    this.id_objeto,
                                    nombre,
                                    valor,
                                    DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
            return SMFactory.getDbh().Execute(query);
        }
        /// <summary>
        /// Adiciona una nueva propiedad al objeto de agenda y serializa su valor en JSON
        /// </summary>
        /// <returns>
        /// The propiedad.
        /// </returns>
        /// <param name='nombre'>
        /// Nombre.
        /// </param>
        /// <param name='valor'>
        /// Valor.
        /// </param>
        public int adicionarPropiedad(string nombre, Hashtable data)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string json = ser.Serialize(data);
            return this.adicionarPropiedad(nombre, json);
        }
        /// <summary>
        /// Actualiza la propieda del objeto de agenda
        /// </summary>
        public void actualizarPropiedad(int id_propiedad, string propiedad, string nuevo_valor)
        {
            Hashtable data = new Hashtable();
            data.Add(propiedad, nuevo_valor);
            Hashtable w = new Hashtable();
            w.Add("id_propiedad", id_propiedad);
            SMFactory.getDbh().update("agenda_objeto_propiedades", data, w);
        }
        public void actualizarPropiedad(int id_propiedad, string propiedad, Hashtable data)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string json = ser.Serialize(data);
            this.actualizarPropiedad(id_propiedad, propiedad, json);
        }
        public object obtenerPropiedad(string propiedad)
        {
            object result = null;

            foreach(Hashtable p in this.propiedades)
            {
                if( !p["propiedad"].ToString().Equals(propiedad) )
                    continue;
                if( (p["valor"].ToString().StartsWith("{") && p["valor"].ToString().EndsWith("}"))
                     || (p["valor"].ToString().StartsWith("[") && p["valor"].ToString().EndsWith("]"))  )
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    result = ser.DeserializeObject(p["valor"].ToString());
                }
                else
                {
                    result = p["valor"];
                }
                break;
            }
            return result;
        }
        /// <summary>
        /// Obtiene todas las propiedades de un objeto con un determinado nombre
        /// </summary>
        /// <returns>
        /// The propiedades.
        /// </returns>
        public ArrayList buscarPropiedades(string propiedad)
        {
            ArrayList result = new ArrayList();
            foreach(Hashtable p in this.propiedades)
            {
                if( p["propiedad"].ToString().Equals(propiedad) )
                {
                    result.Add(p);
                }
            }
            return result;
        }
    }
}
