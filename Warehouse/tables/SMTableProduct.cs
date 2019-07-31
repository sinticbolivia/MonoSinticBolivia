using System;
using SinticBolivia.Database;

namespace SinticBolivia.Warehouse.Tables
{
	public class SBTableProduct : SBTable
	{
		protected long product_id;
		protected string product_code;
		protected string product_name;
		protected string product_description;
		protected long product_line_id;
		protected string product_model;
		protected string product_serial_number;
		protected string product_barcode;
		protected double product_cost;
		protected double product_price;
        protected double product_price_2;
        protected double product_price_3;
		protected double product_price_4;
		protected int product_quantity;
		protected string product_unit_measure;
		protected int store_id;
		protected long author_id;
        protected int min_stock;
        protected string product_internal_code;
		protected string status;
		protected DateTime last_modification_date;
		protected DateTime creation_date;
		
		public SBTableProduct () : base("products", "product_code")
		{
		}
		
	}
}

