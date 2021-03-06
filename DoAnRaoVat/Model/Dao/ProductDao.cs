﻿using Model.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class ProductDao
    {
        DoAnASPDBContext db = null;
        public ProductDao()
        {
            db = new DoAnASPDBContext();
        }

        public long Insert(Product entity)
        {
            db.Products.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }

        public Product GetByID(string Name)
        {
            return db.Products.SingleOrDefault(x => x.Name == Name);
        }

        public Product ViewDetail(int id)
        {
            return db.Products.Find(id);
        }


        public IEnumerable<Product> LissAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Product> model = db.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public bool Update(Product entity)
        {
            try
            {
                var product = db.Products.Find(entity.ID);
                product.Code = entity.Code;
                product.Name = entity.Name;
                //product.ModifiedDate = (DateTime.Now);
                product.Status = entity.Status;
                product.Category_ID = entity.Category_ID;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool Delete(int id)
        {
            var product = db.Products.SingleOrDefault(x => x.ID == id);
            if (product == null)
            {
                return false;
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return true;
        }

        public List<Product> LissAllProduct()
        {
            return db.Products.Where(x => x.Status == true).ToList();
        }

        /*-------------- Thay đổi trạng thái --------------------*/
        public bool ChangeStatus(long id)
        {
            var product = db.Products.Find(id);
            product.Status = !product.Status;
            db.SaveChanges();
            return product.Status;
        }
    }
}
