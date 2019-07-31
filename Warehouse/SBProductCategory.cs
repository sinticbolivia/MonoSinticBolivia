using System;
using System.Collections;
using SinticBolivia.Warehouse.Tables;

namespace SinticBolivia.Warehouse
{
	public class SBProductCategory : SBDBObject
	{
		protected SBTableProductCategories db_table;
		public int CategoryID
		{
			get{return (int)this.GetLong("category_id");}
			set{this.Set("category_id", value);}
		}
		public string CategoryName
		{
			get{return this.GetString("name");}
			set{this.Set("name", value);}
		}
		public string CategoryDescription
		{
			get{return this.GetString("description");}
			set{this.Set("description", value);}
		}
		public int ParentId
		{
			get{return (int)this.GetLong("parent");}
			set{this.Set("parent", value);}
		}
		public DateTime CreationDate
		{
			get{return this.GetDateTime("creation_date");}
			set{this.Set("creation_date", value);}
		}
		public ArrayList Childs
		{
			get{return (ArrayList)this.Data["childs"];}
			set{this.Data["childs"] = value;}
		}
		public SBProductCategory () : base()
		{
			this.db_table = new SBTableProductCategories();
			this.Data.Add("childs", new ArrayList());
		}
        public override void GetDbData(object code_id)
        {
            this.getData(Convert.ToInt32(code_id));
        }
		public void getData(int category_id)
		{
			Hashtable data = this.db_table.getRow("category_id = " + category_id.ToString());
			if( data != null )
				this._dbData = data;
			this.GetChilds();
		}
		public void GetChilds(int category_id)
		{
			this.CategoryID = category_id;
			this.GetChilds();
		}
		public void GetChilds()
		{
			ArrayList childs = this.db_table.getRows(string.Format("parent = {0}", this.CategoryID));
			if( childs != null )
			{
				foreach(Hashtable row in childs)
				{
					SBProductCategory cat = new SBProductCategory();
					cat.SetDbData(row);
					(this.Data["childs"] as ArrayList).Add(cat);
				}

			}
		}
		public void updateData()
		{
			this.db_table.updateRow((Hashtable)this._dbData);
		}
        public ArrayList GetProducts(int store_id = 0)
        {
            ArrayList prods = new ArrayList();
            string query = "";
            if (store_id == 0)
            {
                query = @"SELECT * FROM products p, product2category p2c 
                            WHERE p2c.category_id = {0}
                            AND p.product_id = p2c.product_id";
                query = string.Format(query, this.CategoryID);
            } else
            {
                query = @"SELECT * FROM products p, product2category p2c 
                                    WHERE p.store_id = {0}
                                    AND p2c.category_id = {1}
                                    AND p.product_id = p2c.product_id";
                query = string.Format(query, store_id, this.CategoryID);
            }
            ArrayList _prods = SBFactory.getDbh().Query(query);
            if (_prods != null)
            {
                foreach(Hashtable _p in _prods)
                {
                    SBProduct p = new SBProduct();
                    p.SetDbData(_p);
                    prods.Add(p);
                }

            }

            return prods;
        }
	}
}

