using System;
using System.Collections;

namespace SinticBolivia
{
	[Obsolete("Not used anymore, use SBUser instead", false)]
	public class SBUsuario : SBPersona
	{
		protected int cod_usuario;
        public int CodUsuario
        {
            get{return this.cod_usuario;}
            set{this.cod_usuario = value;}
        }
        protected string login;
        public string Login
        {
            get{return this.login;}
            set{this.login = value;}
        }
        protected string password;
        public string Password
        {
            get{return this.password;}
            set{this.password = value;}
        }
        protected int cod_rol;
        public int CodRol
        {
            get{return this.cod_rol;}
            set{this.cod_rol = value;}
        }
        protected string rol;
        public string Rol
        {
            get{return this.rol;}
            set{this.rol = value;}
        }
        /// <summary>
        /// En esta lista se almacenan todas las capicidades designadas a este usuario basado en su rol.
        /// </summary>
        protected Hashtable capacidades;
		
		public SBUsuario ()
		{
			this.cod_usuario = -1;
            this.capacidades = new Hashtable();
		}
		public SBUsuario(int cod_usuario)
		{
			this.cod_usuario = -1;
            this.capacidades = new Hashtable();
            this.obtenerDatosdeUsuario(cod_usuario);
		}
		public void obtenerDatosdeUsuario(int cod_usuario)
        {
            string query = "SELECT u.cod_usuario, u.login, u.password,u.cod_rol,u.fechahoracreacion,u.estado,u.cod_persona, " +
                            "r.nombre_rol "+
                            "FROM usuario u, roles r " + 
                            "WHERE u.cod_rol = r.cod_rol " +
                            "AND cod_usuario = " + cod_usuario.ToString();
            Hashtable row = SBFactory.getDbh().QueryRow(query);
            if( row != null )
            {
                this.cod_usuario = cod_usuario;
                this.login = row["login"].ToString();
                this.password = row["password"].ToString();
                this.cod_rol = Int32.Parse(row["cod_rol"].ToString());
                this.rol = row["nombre_rol"].ToString();
                //this.obtenerDatos(Int32.Parse(row["cod_persona"].ToString()));
                //obtener capacidades de usuario
                this.obtenerCapacidades();
            }
        }
        public void obtenerDatosDeUsuarioPorCodPersona(int cod_persona)
        {
            string query = "SELECT u.cod_usuario, u.login, u.password,u.cod_rol,u.fechahoracreacion,u.estado,u.cod_persona, " +
                            "r.nombre_rol "+
                            "FROM usuario u, roles r " + 
                            "WHERE u.cod_rol = r.cod_rol " +
                            "AND cod_persona = " + cod_persona.ToString();

            Hashtable row = SBFactory.getDbh().QueryRow(query);
            if( row != null )
            {
                this.cod_usuario = Int32.Parse(row["cod_usuario"].ToString());
                this.login = row["login"].ToString();
                this.password = row["password"].ToString();
                this.cod_rol = Int32.Parse(row["cod_rol"].ToString());
                this.rol = row["nombre_rol"].ToString();
                this.obtenerCapacidades();
            }
        }
        protected void obtenerCapacidades()
        {
            string query = "SELECT id_capacidad, cod_capacidad, nombre_capacidad, llave_capacidad, descripcion_capacidad, objeto_asociado, fecha_creacion " +
                            "FROM capacidades_rol " + 
                            "WHERE cod_rol = " + this.cod_rol.ToString();
            ArrayList caps = SBFactory.getDbh().Query(query);
            if( caps != null )
            {
                foreach(Hashtable cap in caps)
                {
                    this.capacidades.Add(cap["llave_capacidad"], cap);
                }
            }
        }
        /// <summary>
        /// Verifica si el usuario tiene determinada capacidad
        /// </summary>
        /// <returns>
        /// The capacidad.
        /// </returns>
        public bool tieneCapacidadDe(string llave_capacidad)
        {
            if( this.cod_usuario == -1 )
                return false;
            //verificar is es administrador
            if( this.rol.ToLower().IndexOf("admin") != -1 )
                return true;
            if( this.capacidades.ContainsKey(llave_capacidad) )
                return true;
            return false;
        }
        /// <summary>
        /// Verifica si el usuario tiene cierta capacidad y si esta capacidad esta asociada a un determinado objeto
        /// </summary>
        /// <returns>
        /// The capacidad.
        /// </returns>
        /// <param name='llave_capacidad'>
        /// If set to <c>true</c> llave_capacidad.
        /// </param>
        public bool tieneCapacidadDe(string llave_capacidad, string objeto_asociado)
        {
            //verificar si el usuario tiene asignado un codigo de usuario
            if( this.cod_usuario == -1 )
                return false;
            //verificar is es administrador
            if( this.rol.ToLower().IndexOf("admin") != -1 )
                return true;

            if( this.capacidades.ContainsKey(llave_capacidad) )
            {
                Hashtable cap = (Hashtable)this.capacidades[llave_capacidad];
                if( cap["objeto_asociado"].ToString().IndexOf(objeto_asociado) != -1 )
                    return true;

            }   
            return false;
        }
	}
}

