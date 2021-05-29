using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class MerseakUnitTests
    {
        private string _dbName;
        private MearskContext _context;
        protected string DbName => _dbName ??= $"MearskDb";
        #region Setup
        [TestInitialize]
        public void Setup()
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
               ProductName="Book",
               ProductType="Physical",
               SkuCode="A"
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

        decimal GetTotalOrderPrice(List<Orders> orders)
        {
            int remainder, quotient;
            decimal finalPrice = 0;
            List<string> skus = orders.Select(s => s.SKU).ToList();
            foreach (var sku in skus)
            {
                int promoQuantity = _context.ActivePromotions.Where(x => x.SKU == sku).Select(s => s.Quantity).FirstOrDefault();
                int orderQuantity = orders.Where(x => x.SKU == sku).Select(s => s.Quantity).FirstOrDefault();
                if (orderQuantity > promoQuantity && promoQuantity > 0)
                {
                    remainder = orderQuantity % promoQuantity;
                    quotient = orderQuantity / promoQuantity;
                    finalPrice += _context.ActivePromotions.Where(x => x.SKU == sku).Select(s => s.Price).FirstOrDefault() * quotient
                                + _context.SkuList.Where(x => x.SkuCode == sku).Select(s => s.UnitPrice).FirstOrDefault() * remainder;
                }
                else if (orderQuantity == promoQuantity)
                {
                    finalPrice += _context.ActivePromotions.Where(x => x.SKU == sku).Select(s => s.Price).FirstOrDefault();
                }
                else if (orders.Where(x => x.SKU == "D").Count() > 0 && orders.Where(x => x.SKU == "C").Count() > 0)
                {
                    finalPrice += _context.ActivePromotions.Where(x => x.SKU == "C,D").Select(s => s.Price).FirstOrDefault();
                    break;
                }
                else
                {
                    finalPrice += _context.SkuList.Where(x => x.SkuCode == sku).Select(s => s.UnitPrice).FirstOrDefault() * orders.Where(x => x.SKU == sku).Select(s => s.Quantity).FirstOrDefault();
                }
            }


            return finalPrice;
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

        string GetSlipType(string productType, string productName)
        {
            string slipType = string.Empty;
            switch (productType)
            {
                case "Physical":
                    slipType = Constants.Packing;
                    break;
                case "Virtual":
                    slipType = Constants.Email;
                    break;
                default:

                    break;
            }

            switch (productName)
            {
                case "Book":
                    slipType = string.Concat(slipType, $",", Constants.Royalty);
                    break;
                case "Membership":
                    slipType = string.Concat(slipType, $",", Constants.Membership);
                    break;
                case "Video":
                    slipType = string.Concat(slipType, $",", Constants.Firstaid);
                    break;
                default:
                    break;
            }
            return slipType;
        }
        #endregion Setup
      
   
        List<Orders>GetSingleOrderList() 
        {
            List<Orders> singleOrderList = new List<Orders>();
            singleOrderList.Add(new Orders { SKU = "A", Quantity=1});
            singleOrderList.Add(new Orders { SKU = "B", Quantity = 1 });
            singleOrderList.Add(new Orders { SKU = "C", Quantity = 1 });

            return singleOrderList;
        }
        List<Orders> GetMultipleOrderListScenarioB()
        {
            List<Orders> multipleOrderListScenarioB = new List<Orders>();
            multipleOrderListScenarioB.Add(new Orders { SKU = "A", Quantity = 5 });
            multipleOrderListScenarioB.Add(new Orders { SKU = "B", Quantity = 5 });
            multipleOrderListScenarioB.Add(new Orders { SKU = "C", Quantity = 1 });

            return multipleOrderListScenarioB;
        }

        List<Orders> GetMultipleOrderListScenarioC()
        {
            List<Orders> multipleOrderListScenarioC = new List<Orders>();
            multipleOrderListScenarioC.Add(new Orders { SKU = "A", Quantity = 3 });
            multipleOrderListScenarioC.Add(new Orders { SKU = "B", Quantity = 5 });
            multipleOrderListScenarioC.Add(new Orders { SKU = "C", Quantity = 1 });
            multipleOrderListScenarioC.Add(new Orders { SKU = "D", Quantity = 1 });
            return multipleOrderListScenarioC;
        }
        [TestMethod]
        public void SingleOrderShouldBeUnitPriceInSkuList()
        {
            List<Orders> orders = GetSingleOrderList();
            List<string> skus = orders.Select(s => s.SKU).ToList();
            decimal totalSingleItemAmount = _context.SkuList.Where(x=>skus.Contains( x.SkuCode)).Sum(s => s.UnitPrice);
            decimal total = GetTotalOrderPrice(orders);
            Console.WriteLine(total);
            Assert.AreEqual(totalSingleItemAmount, total);
        }

        [TestMethod]
        public void MultipleOrderScenarioBShouldThreeHundredSeventy()
        {

            List<Orders> orders = GetMultipleOrderListScenarioB();
            List<string> skus = orders.Select(s => s.SKU).ToList();
            decimal totalSingleItemAmount = 370;
            decimal total = GetTotalOrderPrice(orders);
            Assert.AreEqual(totalSingleItemAmount, total);

        }


        [TestMethod]
        public void MultipleOrderScenarioBShouldTowHundredEighty()
        {

            List<Orders> orders = GetMultipleOrderListScenarioC();
            List<string> skus = orders.Select(s => s.SKU).ToList();
            decimal totalSingleItemAmount = 280;
            decimal total = GetTotalOrderPrice(orders);
            Assert.AreEqual(totalSingleItemAmount, total);

        }


        [TestMethod]
        [DataRow("A")]
        public void ShouldBePackingAndRoyaltySip(string sku)
        {
          
            var product =_context.Product.Where(x => x.SkuCode == sku).FirstOrDefault();
            string paymentSlip = GetSlipType(product.ProductType, product.ProductName);
            Assert.AreEqual(paymentSlip.Split(",")[0], Constants.Packing);
            Assert.AreEqual(paymentSlip.Split(",")[1], Constants.Royalty);
        }
        
        [TestMethod]
        [DataRow("B")]
        public void ShouldBePackingAndFirstaid(string sku)
        {
            var product =_context.Product.Where(x => x.SkuCode == sku).FirstOrDefault();
            string paymentSlip = GetSlipType(product.ProductType, product.ProductName);
            Assert.AreEqual(paymentSlip.Split(",")[0], Constants.Packing);
            Assert.AreEqual(paymentSlip.Split(",")[1], Constants.Firstaid);
        }

        [TestMethod]
        [DataRow("C")]
        public void ShouldBeEmailAndMembership(string sku)
        {
            var product = _context.Product.Where(x => x.SkuCode == sku).FirstOrDefault();
            string paymentSlip = GetSlipType(product.ProductType, product.ProductName);
            Assert.AreEqual(paymentSlip.Split(",")[0], Constants.Email);
            Assert.AreEqual(paymentSlip.Split(",")[1], Constants.Membership);
        }
    }
}
