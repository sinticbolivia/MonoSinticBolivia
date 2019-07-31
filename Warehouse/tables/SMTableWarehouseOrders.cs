using System;
using SinticBolivia;
using SinticBolivia.Database;

namespace SinticBolivia.Warehouse
{
	public class SBTableWarehouseOrders : SBTable
	{
		protected int order_id;
		protected string order_ref;
		protected string observations;
		protected string order_type;
		protected DateTime shipping_date;
		protected string status;
		protected DateTime creation_date;
		
		public SBTableWarehouseOrders () : base("warehouse_orders", "order_id")
		{
		}
	}
}

