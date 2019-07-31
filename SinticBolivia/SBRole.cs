using System;
using System.Collections;
using SinticBolivia.Database.Tables;

namespace SinticBolivia
{
	public class SBRole : SBDBObject
	{
		protected SBTableRol roles_table;
        protected Hashtable _capabilities;

		public int RoleId
		{
			set{this.Set("role_id", value);}
			get{return this.GetInt("role_id");}
		}
		public string RoleName
		{
			set{this.Set("role_name", value);}
			get{return this.GetString("role_name");}
		}
        public Hashtable Capabilities
        {
            get{return this._capabilities;}
        }
		public SBRole (int role_id = 0) : base()
		{
			this.roles_table = new SBTableRol();
            this._capabilities = new Hashtable();
            if( role_id != 0 )
                this.GetDbData(role_id);
		}
        public override void GetDbData(object code_id)
        {
            this.getData(Convert.ToInt32(code_id));
            if( this.RoleId == 0 )
                return;
            this.GetDbCapabilities();
        }
		public void getData(int role_id)
		{
			Hashtable data = this.roles_table.getRow("role_id = " + role_id.ToString());
            if( data ==  null )
                return;
            this._dbData = data;
			
		}
        public void GetDbCapabilities()
        {
            string query = @"SELECT p.* 
                                FROM permissions p, role2permission r2p 
                                WHERE p.permission_id = r2p.permission_id
                                AND r2p.role_id = {0}";
            query = string.Format(query, this.RoleId);
            ArrayList caps = SBFactory.getDbh().Query(query);
            if( caps == null )
                return;
            foreach(Hashtable cap in caps)
            {
                if( this._capabilities.ContainsKey(cap["permission"]) )
                    continue;
                this._capabilities.Add(cap["permission"], cap);
            }
        }
        public bool HasCapability(string cap)
        {
            if( !this._capabilities.ContainsKey(cap.ToLower()) )
                return false;

            return true;
        }
		public static SBRole getRolByName(string name)
		{
			SBTableRol tr = new SBTableRol();
			Hashtable row = tr.getRow("role_name = '"+name+"'");
			if( row == null )
				return null;
			SBRole rol = new SBRole(Convert.ToInt32(row["role_id"]));
			return rol;
		}
		public static int getRolIdByName(string name)
		{
			SBTableRol tr = new SBTableRol();
			string query = "SELECT rol_id, role_name FROM " + tr.TableName + " WHERE LOWER(role_name) = '"+name.ToLower()+"'";
			Hashtable row = SBFactory.getDbh().QueryRow(query);
			if( row == null )
				return 0;
			return Convert.ToInt32(row["role_id"]);
		}

	}
}

