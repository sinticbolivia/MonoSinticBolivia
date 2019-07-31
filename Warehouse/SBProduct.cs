using System;
using System.Collections;
using SinticBolivia;
using SinticBolivia.Database;
using SinticBolivia.Database.Tables;
using SinticBolivia.Warehouse.Tables;
using System.Security.Cryptography;
using System.Text;

namespace SinticBolivia.Warehouse
{
	public class SBProduct : SBDBObject
	{
		protected SBTableProduct db_table;
        protected Hashtable _meta;
        protected ArrayList _serialNumbers;
		public long Id
		{
			get{return this.GetLong("product_id");}
			set{this.Set("product_id", value);}
		}
		public string Code
		{
			get{return this.GetString("product_code");}
			set{this.Set("product_code", value);}
		}
		public string Name
		{
			get{return this.GetString("product_name");}
			set{this.Set("product_name", value);}
		}
		public string Description
		{
			get{return this.GetString("product_description");}
			set{this.Set("product_description", value);}
		}
		public string BarCode
		{
			get{return this.GetString("product_barcode");}
			set{this.Set ("product_barcode", value);}
		}
		public double Cost
		{
			get{return this.GetDouble("product_cost");}
			set{this.Set ("product_cost", value);}
		}
		public double Price
		{
			get{return this.GetDouble("product_price");}
			set{this.Set ("product_price", value);}
		}
        public double Price2
        {
            get{return this.GetDouble("product_price_2");}
            set{this.Set ("product_price_2", value);}
        }
        public double Price3
        {
            get{return this.GetDouble("product_price_3");}
            set{this.Set ("product_price_3", value);}
        }
		public double Price4
		{
			get{return this.GetDouble("product_price_4");}
			set{this.Set ("product_price_4", value);}
		}
		public int Quantity
		{
			get{return this.GetInt("product_quantity");}
			set{this.Set ("product_quantity", value);}
		}
        public int MinStock
        {
            get{return this.GetInt("min_stock");}
            set{this.Set ("min_stock", value);}
        }
        public string Status
        {
            get{return this.GetString("status");}
            set{this.Set("status", value);}
        }
		/// <summary>
		/// Gets the product image or empty string if product does not has image assigned
		/// </summary>
		public string Image
		{
			get
			{
				SBTableAttachments attach = new SBTableAttachments();
				Hashtable row = attach.getRow("object_type = 'product' AND object_id = '"+this.Id.ToString()+"'");
				if( row == null )
					return String.Empty;
				return row["file"].ToString();
			}
		}
		public int CategoryID
		{
			get
			{
				if( this.Data["category"] != null )
                    return Convert.ToInt32((this.Data["category"] as Hashtable)["category_id"]);
                else
                    return 0;
			}
			set
			{

			}
		}
		public string CategoryName
		{
			get
			{
				if( this.Data["category"] != null )
                    return (this.Data["category"] as Hashtable)["category_name"].ToString();
   				return "";
			}
			set
			{
				//this.db_data["category_name"] = value;
			}
		}
		public int StoreId
        {
            get{ return this.GetInt("store_id");}
            set{this.Set("store_id", value);}
        }
        public ArrayList SerialNumbers
        {
            get{return this._serialNumbers;}
        }
		public SBProduct (object code_id = null) : base()
		{
			this.db_table = new SBTableProduct();
            this._meta = new Hashtable();
            this._serialNumbers = new ArrayList();
            if (code_id != null)
                this.GetDbData(code_id);
		}
        public override void GetDbData(object code_id)
        {
            Hashtable data = null;
            if (code_id is int || code_id is long)
            {
                this.db_table.setPrimaryKey("product_id");
                data = this.db_table.getRow("product_id = " + code_id.ToString());
            } 
            else
            {
                data = this.db_table.getRow("product_code = '"+code_id.ToString()+"'");
            }
                                                
            if( data == null )
                return;

            this._dbData = data;
            //get product meta
            this.GetDbMeta();
            //get serial numbers
            string q = string.Format("SELECT serial_number_id, product_id, serial_number, status FROM product_serialnumbers WHERE product_id = {0}", this.Id);
            ArrayList serials = SBFactory.getDbh().Query(q);
            if (serials != null)
            {
                this._serialNumbers = serials;
            }
            //get category
            string query = "SELECT c.category_id, c.name " +
                            "FROM categories c, product2category p2c " +
                            "WHERE c.category_id = p2c.category_id " +
                            "AND p2c.product_id = "+ this.Id.ToString() +" ";
            Hashtable row = SBFactory.getDbh().QueryRow(query);         
            if( row == null )
                return;
            this.Data.Add("category", row);
            //get store
            //query = string.Format("SELECT * FROM stores WHERE store_id = {0}", this);

        }
		public override void SetDbData(Hashtable data)
		{
			base.SetDbData(data);
			//this.GetDbMeta();
		}
		public void getData(string product_code)
		{   
            this.GetDbData(product_code);
		}
        public void GetDbMeta()
        {
			this._meta = new Hashtable();
            string query = string.Format("SELECT * FROM product_meta WHERE product_code = '{0}'", this.Code);
            ArrayList meta = SBFactory.getDbh().Query(query);
            if( meta == null )
                return;
            foreach(Hashtable r in meta)
            {
                this._meta.Add(r["meta_key"], r["meta_value"]);
            }
        }
        /// <summary>
        /// Gets the product kardex from database
        /// </summary>
        /// <returns >A <see cref="System.Collections.ArrayList"/></returns>
        public ArrayList GetDbKardex(string type = "all")
        {
            string query = string.Format(@"SELECT * FROM product_kardex 
                                            WHERE product_code = '{0}' 
                                            {1}
                                            ORDER BY creation_date ASC 
                                            LIMIT 1000",
                                         this.Id,
                                         (type == "all") ? "" : "AND in_out = '"+ type +"'");
            ArrayList kardex = SBFactory.getDbh().Query(query);
            if( kardex == null )
                return null;
            if( this.Data.ContainsKey("kardex") )
                this.Data["kardex"] = kardex;
            else
                this.Data.Add("kardex", kardex);
            return kardex;
        }
        public ArrayList GetDbKardex(string in_out, DateTime f, DateTime to)
        {
            in_out = in_out.ToLower();
            string query = string.Format(@"SELECT * FROM product_kardex 
                                            WHERE product_code = '{0}' 
                                            {1}
                                            AND DATE(creation_date) >= date('{2}')
                                            AND DATE(creation_date) <= date('{3}')
                                            ORDER BY creation_date",
                                         this.Id,
                                         (in_out == "all") ? "" : "AND in_out = '"+in_out+"'",
                                         f.ToString("yyyy-MM-dd"),
                                         to.ToString("yyyy-MM-dd")
                                         );
            ArrayList kardex = SBFactory.getDbh().Query(query);
            if( kardex == null )
                return null;
            if( this.Data.ContainsKey("kardex") )
                this.Data["kardex"] = kardex;
            else
                this.Data.Add("kardex", kardex);
            return kardex;
        }
        public ArrayList GetDbKardex(DateTime f, DateTime to)
        {
            string query = string.Format(@"SELECT * FROM product_kardex 
                                            WHERE product_code = '{0}' 
                                            AND creation_date >= date('{1}')
                                            AND creation_date <= date('{2}')
                                            ORDER BY creation_date DESC LIMIT 1000",
                                         this.Id,
                                         f.ToString("yyyy-MM-dd"),
                                         to.ToString("yyyy-MM-dd")
                                         );
            ArrayList kardex = SBFactory.getDbh().Query(query);
            if( kardex == null )
                return null;
            this.Data.Add("kardex", kardex);
            return kardex;
        }
        public bool HasSerialNumber(string sn)
        {
            string query = string.Format("SELECT serial_number_id from product_serialnumbers WHERE product_id = {0} AND serial_number = '{1}'",
                                         this.Id, sn);
            return (SBFactory.getDbh().QueryRow(query) == null) ? false : true;
        }
        public object GetMeta(string meta_key)
        {
            if( !this._meta.ContainsKey(meta_key) )
                return null;
            if( string.IsNullOrEmpty(this._meta[meta_key].ToString()) )
                return null;
            return this._meta[meta_key];
        }
        public string GetMetaString(string meta_key)
        {
            return (this.GetMeta(meta_key) != null ) ? this.GetMeta(meta_key).ToString() : "";
        }
        public int GetMetaInt(string meta_key)
        {
            return (this.GetMeta(meta_key) != null ) ? Convert.ToInt32(this.GetMeta(meta_key)) : 0;
        }
		public void Update()
		{
			this.db_table.updateRow(this._dbData);
		}
		
		/// <summary>
		/// Check if  product code already exists into database
		/// </summary>
		/// <param name="code">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool productCodeExists(string code)
		{
			SBProduct p = new SBProduct();
			if( p.db_table.getRow("product_code = '"+code+"'") == null )
				return false;
			return true;
		}
		public static string BuildNewCode()
		{
			string code = "";
			bool exists = true;
			while(exists)
			{
				code = "P-" + SBObject.generateCode();
				exists = SBProduct.productCodeExists(code);
			}
			return code;
		}
        public static string BuildNewEAN13Barcode(string country_code, string manufacturer_code)
        {
            int digits = 12 - (country_code.Length + manufacturer_code.Length);
            string product_code = "";
            string aux = "";
            for(int i = 0; i < digits; i++)
            {
                aux += "9";
            }
            int max_code = Convert.ToInt32(aux);
            for(int i = 0; i < max_code; i++)
            {
                string aux_code = SBUtils.FillLeftCeros(i + 1, digits);
                Ean13 bc = new Ean13(country_code, manufacturer_code, aux_code);
                bc.CalculateChecksumDigit();
                string check_barcode = country_code + manufacturer_code + aux_code + bc.ChecksumDigit;
                string query = string.Format("SELECT product_id FROM products WHERE product_barcode = '{0}'", check_barcode);
                if( SBFactory.getDbh().QueryRow(query) == null )
                {
                    product_code = check_barcode;
                    break;
                }
            }

            return product_code;
        }

	}
}

