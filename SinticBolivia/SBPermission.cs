using System;

namespace SinticBolivia
{
    public class SBPermission
    {
        public SBPermission()
        {
        }
        /// <summary>
        /// Check if a permission already exists into database
        /// </summary>
        /// <returns><c>true</c>, if exists was permissioned, <c>false</c> otherwise.</returns>
        /// <param name="permission">Permission.</param>
        public static bool PermissionExists(string permission)
        {
            string query = "";
            if (SBFactory.getDbh().db_type == "mysql" || SBFactory.getDbh().db_type == "sqlite" || SBFactory.getDbh().db_type == "sqlite3")
                query = string.Format("SELECT permission_id FROM permissions WHERE permission = '{0}' LIMIT 1", permission.Trim().ToLower());
            else if (SBFactory.getDbh().db_type == "sql_server")
            {
                query = string.Format("SELECT TOP 1 permission_id FROM permissions WHERE permission = '{0}'", permission.Trim().ToLower());
            }
            if (SBFactory.getDbh().QueryRow(query) == null)
                return false;

            return true;
        }
    }
}

