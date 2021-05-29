using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class DataSetup
    {
        private string _dbName;
        private MearskContext _context;
        protected string DbName => _dbName ??= $"MearskDb";
        public DataSetup()
        {
            var options = new DbContextOptionsBuilder<MearskContext>()
              .UseInMemoryDatabase(databaseName: DbName)
              .EnableSensitiveDataLogging(true)
              .Options;
            _context = new MearskContext(options);
            ClearData();
            SetupSKUList();
            SetupActivePromotions();
            SetupProduct();
        }
        void SetupSKUList()
        {
            var skuA = new SKUList
            {
                SkuCode = "A",
                UnitPrice = 50
            };
            var skuB = new SKUList
            {
                SkuCode = "B",
                UnitPrice = 30
            };
            var skuC = new SKUList
            {
                SkuCode = "C",
                UnitPrice = 20
            };
            var skuD = new SKUList
            {
                SkuCode = "D",
                UnitPrice = 15
            };
            List<SKUList> skuList = new List<SKUList>();
            skuList.Add(skuA);
            skuList.Add(skuB);
            skuList.Add(skuC);
            skuList.Add(skuD);

            _context.SkuList.AddRange(skuList);
            _context.SaveChanges();

        }
        void SetupActivePromotions()
        {
            var activePromotion1 = new ActivePromotions
            {

                SKU = "A",
                Quantity = 3,
                Price = 130,
                Type = "number"
            };
            var activePromotion2 = new ActivePromotions
            {

                SKU = "B",
                Quantity = 2,
                Price = 45,
                Type = "number"
            };
            var activePromotion3 = new ActivePromotions
            {

                SKU = "C,D",
                Quantity = 0,
                Price = 30,
                Type = "SKU"
            };

            List<ActivePromotions> activePromotions = new List<ActivePromotions>();
            activePromotions.Add(activePromotion1);
            activePromotions.Add(activePromotion2);
            activePromotions.Add(activePromotion3);

            _context.ActivePromotions.AddRange(activePromotions);
            _context.SaveChanges();

        }
        void SetupProduct()
        {
            var product1 = new Product
            {
                ProductName = "Book",
                ProductType = "Physical",
                SkuCode = "A"
            };
            var product2 = new Product
            {
                ProductName = "Video",
                ProductType = "Physical",
                SkuCode = "B"
            };
            var product3 = new Product
            {
                ProductName = "Membership",
                ProductType = "Virtual",
                SkuCode = "C"
            };
            List<Product> products = new List<Product>();
            products.Add(product1);
            products.Add(product2);
            products.Add(product3);

            _context.Product.AddRange(products);
            _context.SaveChanges();
        }

        void ClearData()
        {
            var existingSKU = _context.SkuList.ToList();
            if (existingSKU.Count > 0)
            {
                _context.SkuList.RemoveRange(existingSKU);
            }

            var existingActivePromo = _context.ActivePromotions.ToList();
            if (existingActivePromo.Count > 0)
            {
                _context.ActivePromotions.RemoveRange(existingActivePromo);
            }
        }

    }
}
