using System;
using System.Collections;
using SinticBolivia.Database;
using SinticBolivia.Warehouse.Tables;

namespace SinticBolivia.Warehouse
{
	public class SBSale : SBTransaction
	{
		public SBSale () : base()
		{
			
		}
		/// <summary>
		/// Add new sale details
		/// Implemented columns
		///  col: product_code
		///  col: quantity
		/// </summary>
		/// <param name="detail_row">
		/// A <see cref="Hashtable"/>
		/// </param>
		public void addDetail(Hashtable detail_row)
		{
		}
	}
}

