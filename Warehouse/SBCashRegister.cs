using System;
using System.Collections;
using System.Globalization;
using SinticBolivia.Database;

namespace SinticBolivia.Warehouse
{
	public class SBCashRegister
	{
		public SBCashRegister ()
		{
			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="opening_balance">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <param name="currency_id">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="user_id">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public static int open(decimal opening_balance, int currency_id, int user_id, int terminal_id, int branch_id)
		{
			Console.WriteLine("opening cash");
			Hashtable row = new Hashtable();
			row.Add("opening_balance", opening_balance);
			row.Add("ending_balance", 0);
			row.Add("currency_id", currency_id);
			row.Add("user_id", user_id);
			row.Add("terminal_id", terminal_id);
			row.Add("branch_id", branch_id);
			row.Add("status", "open");
			row.Add("creation_date", DateTime.Now);
			SBFactory.getDbh().insert("cash_register", row);
			return (int)(SBFactory.getDbh() as SBDatabase).LastInsertID;
		}
		public static void close(float ending_balance, int terminal_id, int branch_id, DateTime date)
		{
			if( !SBCashRegister.isOpen(terminal_id, branch_id, date))
			{
				throw new Exception("Cash Register is not open");
			}
			
			string query = "UPDATE cash_register " +
							"SET AND status = 'closed', "+
							"ending_balance = {0} "+
							"WHERE terminal_id = {1} " + 
							"AND branch_id = {2} " +
							"AND DATE(creation_date) = '{3}'" + 
							"AND status = 'open'";
			query = String.Format(query, ending_balance, terminal_id, branch_id, date.ToString("yyyy-MM-dd"));
			SBFactory.getDbh().Query(query);
			
		}
		public static bool isOpen(int terminal_id, int branch_id, DateTime date)
		{
			string datetime_format = CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern;
			string query = "SELECT * FROM cash_register " +
							"WHERE terminal_id = {0} " +
							"AND branch_id = {1} " +
							"AND status = 'open' "+
							"AND DATE(creation_date) = '{2}'";
			query = String.Format(query, terminal_id, branch_id, date.ToString("yyyy-MM-dd"));
			//Console.WriteLine();
			Hashtable row = SBFactory.getDbh().QueryRow(query);
			if( row == null )
				return false;
			return true;
		}
	}
}

