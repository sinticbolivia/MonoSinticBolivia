using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace SinticBolivia
{
	public class SBPersona : SBObject
	{
		public int codpersona;
        public string nombre;
        public DateTime fechanacimiento;
        public string direccion;
        public string telefono;
        public string email;
        public string nitci;
        public string estado;
        public int codtipopersona;
		
		public SBPersona ()
		{
		}
		public void obtenerDatos(int codpersona)
        {
            string query = "SELECT "+ this.getPropertiesWithGlue(",") + " " +
            				"FROM persona " +
            				"WHERE codpersona = " + codpersona;
            Hashtable data = SBFactory.getDbh().QueryRow(query);
            if( data == null )
                return;
            this.bind(data);
        }
	}
}

