using System;
using System.Collections;
using SinticBolivia.Warehouse.Tables;

namespace SinticBolivia.Warehouse
{
	public class SBTransaction : SBDBObject
	{
		protected SBTableTransactions db_table;
		protected ArrayList _items;
		protected Hashtable _meta;
        public SBTransactionType TransactionType;
        public SBUser User;

        public int Id
        {
            get{return this.GetInt("transaction_id");}
        }
		public string TransactionCode
		{
			get{return this.GetString("transaction_code");}		
			//set{this.db_data["transaction_code"] = value;}
		}
		public int TransactionTypeId
		{
			get{return this.GetInt("transaction_type_id");}		
			set{this.Set("transaction_type_id", value);}
		}
		public int StoreId
		{
			get{return this.GetInt("store_id");}		
			set{this.Set("store_id", value);}
		}
		public int UserId
		{
			get{return this.GetInt("user_id");}		
			set{this.Set("user_id", value);}
		}
        /// <summary>
        /// The customer or supplier id
        /// </summary>
        /// <value>The owner code identifier.</value>
        public string OwnerCodeId
        {
            get{return this.GetString("owner_code_id");}     
            set{this.Set("owner_code_id", value);}
        }
		public string Details
		{
			get{return this.GetString("details");}		
            set{this.Set("details", value);}
		}
		public float Subtotal
		{
			get{return this.GetSingle("sub_total");}		
            set{this.Set("sub_total", value);}
		}
        public float Discount
        {
            get{return this.GetSingle("discount");}        
            set{this.Set("discount", value);}
        }
        public float Total
        {
            get{return this.GetSingle("total");}        
            set{this.Set("total", value);}
        }
		public string Status
		{
			get{return this.GetString("status");}		
			set{this.Set("status", value);}
		}
        public int Sequence
        {
            get{return this.GetInt("sequence");}       
        }
        public int TotalItems
        {
            get{return this.GetInt("total_items");}       
        }
        public DateTime LastModificationDate
        {
            get{return this.GetDateTime("last_modification_date");}      
            set{this.Set("last_modification_date", value);}      
        }
		public DateTime CreationDate
		{
			get{return this.GetDateTime("creation_date");}		
		}
		public ArrayList Items
        {
            get{return this._items;}
        }
        public Hashtable Meta
        {
            get{return this._meta;}
        }
		public SBTransaction (string code = "") : base()
		{
			this.db_table = new SBTableTransactions();
            this._items = new ArrayList();
            this._meta = new Hashtable();
            if( !string.IsNullOrEmpty(code) )
                this.GetDbData(code);
		}
		public override void GetDbData(object transaction_code)
		{
			Hashtable row = (transaction_code is int) ? this.db_table.getRow("transaction_id = "+ transaction_code.ToString()) : 
                                                        this.db_table.getRow("transaction_code = '"+ transaction_code.ToString()+"'");
			if( row == null )
				return;
            this._dbData = row;
            if (this.UserId > 0)
            {
                this.User = new SBUser(this.UserId);
            }
			//get details
			this.GetDbItems(this.TransactionCode);
            this.GetDbMeta(this.TransactionCode);
            this.TransactionType = new SBTransactionType(this.TransactionTypeId);
		}
        public override void SetDbData(Hashtable data)
        {
            base.SetDbData(data);
            if (this.UserId > 0)
                this.User = new SBUser(this.UserId);
        }
		public void GetDbItems(string transaction_code)
		{
            string query = string.Format("SELECT * FROM transaction_items WHERE transaction_code = '{0}'", transaction_code);
            Console.WriteLine(query);
            ArrayList items = SBFactory.getDbh().Query(query);
            if( items == null )
                return;
            this._items = items;
		}
        public void GetDbMeta(string transaction_code)
        {
            string query = string.Format("SELECT * FROM transaction_meta WHERE transaction_code = '{0}'", transaction_code);
            ArrayList meta = SBFactory.getDbh().Query(query);
            if( meta == null )
                return;
            foreach(Hashtable row in meta)
            {
                this._meta.Add(row["meta_key"], row["meta_value"]);
            }
        }
        /// <summary>
        /// Reverts the transaction.
        /// </summary>
        /// <returns><c>true</c>, if transaction was reverted, <c>false</c> otherwise.</returns>
        public bool RevertTransaction()
        {
            this.Status = "reverted";
            //Hashtable w = new Hashtable();
            //w.Add("transaction_code", this.TransactionCode);

            SBFactory.getDbh().BeginTransaction();

            //SBFactory.getDbh().update("transactions", this._dbData, w);
            if (this.TransactionType.InputOutput.ToUpper() == "IN" || this.TransactionType.InputOutput.ToUpper() == "INPUT")
            {
                //reduce products stock
                foreach(Hashtable item in this._items)
                {
                    int product_id = Convert.ToInt32(item["object_id"]);
                    SBProduct product = new SBProduct(product_id);
                    product.Quantity -= Convert.ToInt32(item["object_quantity"]);
                    product.Update();
                    //delete transaction product kardex records
                    SBFactory.getDbh().delete("product_kardex", new{product_code = product.Id.ToString(), transaction_code = this.TransactionCode});
                }
            }
            else if( this.TransactionType.InputOutput.ToUpper() == "OUT" || this.TransactionType.InputOutput.ToUpper() == "OUTPUT" )
            {
                //increase products stock
                foreach(Hashtable item in this._items)
                {
                    string product_code = item["object_code"].ToString();
                    SBProduct product = new SBProduct();
                    product.GetDbData(product_code);
                    product.Quantity += Convert.ToInt32(item["object_quantity"]);
                    product.Update();
                    //delete transaction product kardex records
                    SBFactory.getDbh().delete("product_kardex", new{product_code = product.Id.ToString(), transaction_code = this.TransactionCode});
                }
            }
            this.db_table.updateRow(this._dbData);
            SBFactory.getDbh().EndTransaction();
            return true;
        }
		public static string BuildNewTransactionCode(int transaction_type_id, string prefix)
		{
			bool valid_code = false;
			string fcode = "{0}{1}";
            string code = "";
			while( !valid_code )
			{
                code = string.Format(fcode, prefix, SBObject.generateCode());
				string query = "SELECT transaction_id FROM transactions WHERE transaction_type_id = "+transaction_type_id.ToString()+"" +
                                " AND transaction_code = '"+code+"'";
				Hashtable row = SBFactory.getDbh().QueryRow(query);
				if( row == null )
                {
                    valid_code = true;
                }
				
			}
			return code;
			
		}
        public static long GetNextSequenceTransaction(long transaction_type_id, long store_id)
        {
            string query = string.Format("SELECT COUNT(sequence) AS sequence FROM transactions WHERE transaction_type_id = {0} AND store_id = {1}", 
                                         transaction_type_id, store_id);
            Hashtable row = SBFactory.getDbh().QueryRow(query);
            if( row == null )
                return 1;
            long sequence = Convert.ToInt64(row["sequence"]);

            return (sequence + 1);
        }
        public static string SequenceToString(int sequence)
        {
            string ss = "";
            for(int i = 0; i < (8 - (sequence.ToString().Length)); i++)
            {
                ss += "0";
            }
            ss += sequence.ToString();

            return ss;
        }
		/// <summary>
		/// Complete an INPUT/OUTPUT transaction
		/// </summary>
		/// <returns><c>true</c>, if transaction was completed, <c>false</c> otherwise.</returns>
		/// <param name="transaction_code">Transaction code.</param>
		/// <param name="apply_price_rules">If set to <c>true</c> apply price rules.</param>
        public static bool CompleteTransaction(string transaction_code, bool apply_price_rules = false)
        {
            try
            {
                SBTransaction transaction = new SBTransaction(transaction_code);
                Hashtable tt = SBWarehouse.GetTransactionType(transaction.TransactionTypeId);
                //check if it's  an input
                if( tt["in_out"].ToString().ToUpper() == "IN" || tt["in_out"].ToString().ToUpper() == "INPUT" )
                {
                    //update stocks
                    foreach(Hashtable item in transaction.Items)
                    {
                        SBProduct prod 	= new SBProduct();
                        prod.GetDbData(item["object_code"]);
                        prod.Quantity 	= prod.Quantity + Convert.ToInt32(item["object_quantity"]);
                        prod.Cost 		= Convert.ToDouble(item["object_price"]);
                        /*
                        //check to apply price rules
                        if( apply_price_rules )
                        {
                            //update product prices
                            double _price = SBWarehouse.GetPriceFromPriceRule((float)prod.Cost, "price_1");
                            if( _price != prod.Cost )
                                prod.Price = _price;
                            _price = SBWarehouse.GetPriceFromPriceRule((float)prod.Cost, "price_2");
                            if( _price != prod.Cost )
                                prod.Price2 = _price;
                            //we need to round decimals to next integer for prices
                            prod.Price = Math.Ceiling(prod.Price);
                            prod.Price2 = Math.Ceiling(prod.Price2);
                        }
                        else
                        {
                        }
                        */
                        //string p_code = prod.Code;
                        prod.Update();

						string real_cost_str = SBMeta.GetMeta("transaction_item_meta", 
							"transaction_item_id", Convert.ToInt32(item["transaction_item_id"]),
							"_real_cost"
						).ToString();
						float real_cost = 0;
						float.TryParse(real_cost_str, out real_cost);
						SBMeta.UpdateMeta("product_meta", 
							"product_code", prod.Code, 
							"_real_cost", real_cost.ToString("0.00")
						);
						//##get latest two cost of the product for weighted average inventory
						float weighted_cost = 0;
						string query = "SELECT unit_price FROM product_kardex "+
										"WHERE transaction_type_id = {0} " + 
										"AND product_code = '{1}' " +
										"ORDER BY creation_date DESC " + 
										"LIMIT 2";
						query = string.Format(query, transaction.TransactionTypeId, item["object_id"].ToString());
						//Console.WriteLine(query);
						var costs = SBFactory.getDbh().Query(query);
						if( costs != null )
						{
							float total_cost = 0;
							foreach(Hashtable row in costs)
							{
								total_cost += Convert.ToSingle(row["unit_price"]);
							}
							weighted_cost = total_cost / 2;
						}
						//##get fifo cost
						float cost_fifo = 0;
						//##build data to create product kardex
                        Hashtable k = new Hashtable();
                        k.Add("product_code", prod.Id);
                        k.Add("quantity", item["object_quantity"]);
                        k.Add("quantity_balance", prod.Quantity);
                        k.Add("unit_price", item["object_price"]);
                        k.Add("cost", item["object_price"]);
						k.Add("cost_fifo", cost_fifo);
						k.Add("cost_weighted_average", weighted_cost);
                        k.Add("total_amount", item["total"]);
                        k.Add("monetary_balance", prod.Quantity * Convert.ToSingle(item["object_price"]));
                        k.Add("author_id", SBUser.getLoggedInUser().UserId);
                        k.Add("creation_date", DateTime.Now);
                        k.Add("transaction_code", transaction.TransactionCode);
                        if( prod.Status.ToLower() == "initial" )
                        {
                            //create initial kardex
                            k.Add("in_out", "initial");
                            k.Add("transaction_type_id", -1);
                            prod.Status = "publish";
                            prod.Update();
                        }
                        else
                        {
                            //update product kardex
                            k.Add("in_out", "input");
                            k.Add("transaction_type_id", tt["transaction_type_id"]);
                        }
                        SBFactory.getDbh().insert("product_kardex", k);

                    }
                    //update transaction status to completed|received
                    string tquery = string.Format("UPDATE transactions SET status = 'received' WHERE transaction_code = '{0}'", 
                                                  transaction.TransactionCode);
                    SBFactory.getDbh().Execute(tquery);
                }
				//##complete OUTPUT transaction
                else
                {
                    foreach(Hashtable item in transaction.Items)
                    {
						//##update product stock
                        SBProduct prod = new SBProduct(Convert.ToInt32(item["object_id"]));
                        prod.Quantity = prod.Quantity - Convert.ToInt32(item["object_quantity"]);
                        string p_code = prod.Code;
                        prod.Update();
                        //##update product kardex
                        Hashtable k = new Hashtable();
                        k.Add("product_code", prod.Id);
                        k.Add("in_out", "output");
                        k.Add("quantity", item["object_quantity"]);
                        k.Add("quantity_balance", prod.Quantity);
                        k.Add("cost", prod.Cost);
						k.Add("cost_fifo", 0); //TODO: calculated fifo cost
						k.Add("cost_weighted_average", 0); //TODO: calculate average cost
						k.Add("unit_price", Convert.ToSingle(item["object_price"]));
                        k.Add("total_amount", item["sub_total"]);
                        k.Add("monetary_balance", prod.Quantity * prod.Cost);
                        k.Add("transaction_type_id", tt["transaction_type_id"]);
                        k.Add("author_id", SBUser.getLoggedInUser().UserId);
                        k.Add("creation_date", DateTime.Now);
                        k.Add("transaction_code", transaction.TransactionCode);
                        SBFactory.getDbh().insert("product_kardex", k);
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                (SBGlobals.getVar("app") as SBApplication).logString(e.Message);
                (SBGlobals.getVar("app") as SBApplication).logString(e.StackTrace);
                throw e;
            }
        }
        /// <summary>
        /// Reverts the transaction.
        /// </summary>
        /// <returns><c>true</c>, if transaction was reverted, <c>false</c> otherwise.</returns>
        /// <param name="transaction_code">Transaction_code.</param>
        public static bool RevertTransaction(string transaction_code)
        {
            SBTransaction t = new SBTransaction(transaction_code);
            return t.RevertTransaction();
        }
	}
}

