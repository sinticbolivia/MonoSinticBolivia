using System;
using System.Collections.Generic;
using System.Collections;
using SinticBolivia.Warehouse.Tables;
using SinticBolivia.Database;
using SinticBolivia.Database.Tables;

namespace SinticBolivia.Warehouse
{
	public class SBWarehouse
	{
		public SBWarehouse ()
		{
			
		}
		/// <summary>
		/// Gets the categories.
		/// The method return an array list of SBProductCategory objects
		/// </summary>
		/// <returns>
		/// The categories., <see cref="SBProductCategory" />
		/// </returns>
		public static ArrayList getCategories(int store_id = 0)
		{
			var tc = new SBTableProductCategories();
			var rows = tc.getRows("parent = 0");
			if( rows == null )
				return null;
			var cats = new ArrayList();
			foreach(Hashtable row in rows)
			{
				SBProductCategory cat = new SBProductCategory();
				cat.SetDbData(row);
				cats.Add(cat);
			}
			return cats;
		}
		public static long AddCategory(Hashtable row)
		{
			SBTableProductCategories ct = new SBTableProductCategories();
			ct.insertRow(row);
			return ct.LastInsertID;
		}
		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <returns>
		/// The category <see cref="SBProductCategory"/>.
		/// </returns>
		/// <param name='category_id'>
		/// Category_id.
		/// </param>
		public static SBProductCategory GetCategory(int category_id)
		{
			SBProductCategory cat = new SBProductCategory();
			cat.GetDbData(category_id);

			return cat;
		}
		/// <summary>
		/// Gets the category products.
		/// The method returns an SBProduct object array
		/// </summary>
		/// <returns>The category products.</returns>
		/// <param name="category_id">Category identifier.</param>
        public static ArrayList GetCategoryProducts(int category_id)
        {
            string query = "SELECT p.* "+
							"FROM products p, product2category p2c "+
							"WHERE p.product_id = p2c.product_id "+
							"AND p2c.category_id = {0}";
            query = string.Format(query, category_id);
            ArrayList prods = SBFactory.getDbh().Query(query);
            ArrayList obj_prods = new ArrayList();
            foreach(Hashtable p in prods)
            {
                SBProduct product = new SBProduct();
                product.SetDbData(p);
                obj_prods.Add(product);
            }
            
            return obj_prods;
        }
		/// <summary>
		/// Get all Warehouse products and return an ArrayList that contains SMProduct instances
		/// </summary>
		/// <returns>
		/// A <see cref="ArrayList"/>
		/// </returns>
		public static ArrayList getProducts()
		{
			SBTableProduct tp = new SBTableProduct();
			ArrayList prods = tp.getRows("(status = 'publish' OR status = 'initial') ORDER BY creation_date ASC");
			if( prods == null )
				return null;
			ArrayList obj_prods = new ArrayList();
			foreach(Hashtable p in prods)
			{
				SBProduct product = new SBProduct();
				product.SetDbData(p);
				obj_prods.Add(product);
			}
			
			return obj_prods;
		}
		public static ArrayList getProducts(int category_id)
		{
			return SBWarehouse.GetCategoryProducts(category_id);
		}
		/// <summary>
		/// Get warehouse product
		/// </summary>
		/// <param name="product_code">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="SMProduct"/>
		/// </returns>
		public static SBProduct getProduct(string product_code)
		{
			SBProduct prod = new SBProduct();
			prod.getData(product_code);
			return prod;
		}
        /// <summary>
        /// Gets the product by codebar.
        /// </summary>
        /// <returns>The product by codebar.</returns>
        public static SBProduct GetProductByCodebar(string barcode, int store_id = 0)
        {
            SBFactory.getDbh().Select("*")
                                .From("products")
                                .Where(string.Format("product_barcode = '{0}'", barcode));
            if (store_id > 0)
            {
                SBFactory.getDbh().And(string.Format("store_id = {0}", store_id));
            }

            Hashtable prod_row = SBFactory.getDbh().QueryRow();
            if (prod_row == null)
                return null;
            SBProduct product = new SBProduct();
            product.SetDbData(prod_row);

            return product;
        }
		/// <summary>
		/// Descrease the product stock in warehouse
		/// </summary>
		/// <param name="product_code">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="qty">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="SMProduct"/>
		/// </returns>
		public static SBProduct restStock(string product_code, int qty)
		{
			SBProduct p = SBWarehouse.getProduct(product_code);
			p.Quantity = p.Quantity - qty;
			p.Quantity = (p.Quantity < 0) ? 0 : p.Quantity;
			p.Update();
			return p;
		}
		/// <summary>
		/// Increase the product stock in warehouse
		/// </summary>
		/// <param name="product_code">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="qty">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="SMProduct"/>
		/// </returns>
		public static SBProduct increaseProductStock(string product_code, int qty)
		{
			SBProduct p = SBWarehouse.getProduct(product_code);
			p.Quantity = p.Quantity + qty;
			p.Update();
			return p;
		}
		/// <summary>
		/// Get all available warehouse suppliers and return them as SBSupplier object
		/// </summary>
		/// <returns>
		/// A <see cref="ArrayList"/>
		/// </returns>
		public static ArrayList getSuppliers()
		{
			SBTableSuppliers ts = new SBTableSuppliers();
			ArrayList rows = ts.getRows("");
			if( rows == null )
				return null;
			ArrayList suppliers = new ArrayList();
			foreach(Hashtable row in rows)
			{
				SBSupplier supplier = new SBSupplier();
				supplier.getData(Convert.ToInt32(row["supplier_id"]));
				suppliers.Add(supplier);
			}
			return suppliers;
			
		}
		/// <summary>
		/// Pleace a new warehouse order with status pending (default status)
		/// </summary>
		/// <param name="reference">
		/// A order reference <see cref="System.String"/>
		/// </param>
		/// <param name="obs">
		/// A observations <see cref="System.String"/>
		/// </param>
		/// <param name="order_type">
		/// A incoming|outgoing|transfer <see cref="System.String"/>
		/// </param>
		/// <param name="shipping_date">
		/// A <see cref="DateTime"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public static int placeNewOrder(string reference, string obs, string order_type, DateTime shipping_date)
		{
			SBTableWarehouseOrders order = new SBTableWarehouseOrders();
			Hashtable row = new Hashtable();
			row.Add("order_ref", reference);
			row.Add("observations", obs);
			row.Add("order_type", order_type);
			row.Add("shipping_date", shipping_date);
			row.Add("status", "pending");
			row.Add("creation_date", DateTime.Now);
			
			return order.insertRow(row);
		}
        /// <summary>
        /// Gets the transaction types.
        /// </summary>
        /// <returns>
        /// The transaction types. A <see cref="System.Collections.ArrayList"/> with rows as <see cref="System.Collections.Hashtable"/>
        /// </returns>
		public static ArrayList GetTransactionTypes()
		{
			SBTableTransactionType tt = new SBTableTransactionType();
			ArrayList tts = tt.getRows("");

			return tts;
		}
        public static List<SBTransactionType> GetTransactionTypes(bool objects = true)
        {
            List<SBTransactionType> tts = new List<SBTransactionType>();
            ArrayList _tts = SBWarehouse.GetTransactionTypes();
            if (_tts == null)
                return null;
            foreach(Hashtable _tt in _tts)
            {
                SBTransactionType tt = new SBTransactionType();
                tt.SetDbData(_tt);
                tts.Add(tt);
            }

            return tts;
        }
        public static Hashtable GetTransactionType(int id)
        {
            SBTableTransactionType tt = new SBTableTransactionType();
            Hashtable row = tt.getRow(string.Format("transaction_type_id = {0}", id));
            return row;
        }
		public static long AddTransactionType(Hashtable row)
		{
			SBTableTransactionType tt = new SBTableTransactionType();
			tt.insertRow(row);
			return tt.LastInsertID;
		}
		public static long AddNewStore(Hashtable row)
		{
			SBTableStores store = new SBTableStores();
			store.insertRow(row);

			return store.LastInsertID;
		}
        /// <summary>
        /// Gets the stores.
        /// The method returns database rows as Hashtables
        /// </summary>
        /// <returns>
        /// A <see cref="ArrayList"/>
        /// </returns>
		public static ArrayList GetStores()
		{
			SBTableStores store = new SBTableStores();
			return store.getRows("");
		}
		public static Hashtable GetStore(int store_id)
		{
			SBTableStores store = new SBTableStores();
			return store.getRow(string.Format("store_id = {0}", store_id));
		}
        public static bool StoreProductExists(int store_id, string product_code)
        {
            SBTableProduct tp = new SBTableProduct();
            Hashtable prod = tp.getRow(string.Format("store_id = {0} AND product_code = '{1}'", store_id, product_code.Trim()));
            return (prod == null || prod.Count <= 0) ? false : true;
        }
        /// <summary>
        /// Gets the store product.
        /// </summary>
        /// <returns>
        /// The store product.
        /// A <see cref="SBProduct"/>
        /// </returns>
        /// <param name='store_id'>
        /// Store_id.
        /// </param>
        /// <param name='product_code'>
        /// Product_code.
        /// </param>
        public static SBProduct GetStoreProduct(int store_id, string product_code)
        {
            SBTableProduct tp = new SBTableProduct();
            Hashtable prod = tp.getRow(string.Format("store_id = {0} AND product_code = '{1}'", store_id, product_code.Trim()));
            if( prod == null )
                return null;
            SBProduct product = new SBProduct();
            product.SetDbData(prod);
            product.GetDbMeta();
            return product;
        }
        public static ArrayList GetStoreProducts(int store_id)
        {
            SBTableProduct tp = new SBTableProduct();
            ArrayList _prods = tp.getRows(string.Format("(status = 'publish' OR status = 'initial') AND store_id = {0}", store_id));
            if( _prods == null )
                return null;
            ArrayList prods = new ArrayList();
            foreach(Hashtable prod in _prods)
            {
                SBProduct product = new SBProduct();
                product.SetDbData(prod);
                product.GetDbMeta();
                prods.Add(product);
            }

            return prods;
        }
        public static ArrayList GetStoreCategoryProducts(int store_id, int category_id)
        {
            string query = @"SELECT p.* FROM products p, product2category p2c
                                WHERE p.product_id = p2c.product_id
                                AND p.store_id = {0}
                                AND p2c.category_id = {1}";
            query = string.Format(query, store_id, category_id);
            ArrayList _prods = SBFactory.getDbh().Query(query);
            if( _prods == null )
                return null;
            ArrayList prods = new ArrayList();
            foreach(Hashtable prod in _prods)
            {
                SBProduct product = new SBProduct();
                product.SetDbData(prod);
                product.GetDbMeta();
                prods.Add(product);
            }

            return prods;
        }
		public static ArrayList GetProductLines()
		{
			SBTableProductLines plt = new SBTableProductLines();
			return plt.getRows("");
		}
		public static int AddProductLine(Hashtable row)
		{
			SBTableProductLines plt = new SBTableProductLines();
			plt.insertRow(row);
			return (int)plt.LastInsertID;
		}
		public static bool DeleteProductLine(int line_id)
		{
			SBTableProductLines plt = new SBTableProductLines();
			return plt.DeleteRow("line_id", line_id);
		}
		public static Hashtable GetProductLine(int line_id)
		{
			SBTableProductLines plt = new SBTableProductLines();
			return plt.getRow(string.Format("line_id = {0}", line_id));
		}
        public static ArrayList GetPriceRules(int category_id, string price_number)
        {
            string query = "";
			/*
            if (SBFactory.getDbh().db_type == "sqlite" || SBFactory.getDbh().db_type == "sqlite3")
            {
                query = string.Format("SELECT [rule_id], [price], [from], [to], [percentage], [creation_date] FROM price_rules WHERE category_id = {0} AND price = '{1}'", 
                                         category_id, price_number);
            } 
            else
            {
                query = string.Format("SELECT rule_id, price, from, to, percentage, creation_date FROM price_rules WHERE category_id = {0} AND price = '{1}'", 
                                         category_id, price_number);
            }
            */
			query = string.Format("SELECT * FROM price_rules WHERE category_id = {0} AND price = '{1}'", 
				category_id, price_number);
            ArrayList rules = SBFactory.getDbh().Query(query);
            return rules;
        }
		/// <summary>
		/// Gets the price from price rule.
		/// </summary>
		/// <returns>The price from price rule.</returns>
		/// <param name="cost">Cost.</param>
		/// <param name="category_id">Category identifier.</param>
		/// <param name="price_number">Price number.</param>
        public static float GetPriceFromPriceRule(float cost, int category_id, string price_number)
        {
            ArrayList rules = SBWarehouse.GetPriceRules(category_id, price_number);
            if( rules == null )
                return cost;
			string param = string.Format("cat_{0}_rounding", category_id);
			string rounding = (SBGlobals.getVar("app") as SBApplication).getParameter(param, "off").ToString();
            float the_price = cost;
          
            foreach(Hashtable rule in rules)
            {
                float cost_from 	= Convert.ToSingle(rule["from"]);
                float cost_to 		= Convert.ToSingle(rule["to"]);
                float percentage 	= Convert.ToSingle(rule["percentage"]);
          
				the_price = cost + (cost * percentage);
                if( cost > cost_from && cost_to == -1 )
                {
					if (the_price > 100 && rounding == "on") 
					{
						Console.WriteLine("Price: {0}", the_price);
						string price_str = ((int)the_price).ToString();
						int last_digit = int.Parse(price_str[price_str.Length - 1].ToString());
						if (last_digit > 0) 
						{
							the_price += (10 - last_digit);
						}
						the_price = (int)the_price;
						Console.WriteLine("New Price: {0}", the_price);
					} 
					else 
					{
						the_price = (float)Math.Ceiling(the_price);
					}
                    break;
                }
                else if( cost > cost_from && cost <= cost_to )
                {
					if (the_price > 100 && rounding == "on") 
					{
						Console.WriteLine("Price: {0}", the_price);
						string price_str = ((int)the_price).ToString();
						int last_digit = int.Parse(price_str[price_str.Length - 1].ToString());
						if (last_digit > 0) 
						{
							the_price += (10 - last_digit);
						}
						the_price = (int)the_price;
						Console.WriteLine("New Price: {0}", the_price);
					}
					else 
					{
						the_price = (float)Math.Ceiling(the_price);
					}
                    break;
                }

            }
            //Console.WriteLine("{0} => {1}, ({2})", cost, the_price, percentage);
			return the_price;
        }
	}
}

