using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromotionEngine
{
    public class PromoCalculator
    {
        private string _dbName;
        private MearskContext _context;
        protected string DbName => _dbName ??= $"MearskDb";
        public PromoCalculator()
        {
            var options = new DbContextOptionsBuilder<MearskContext>()
              .UseInMemoryDatabase(databaseName: DbName)
              .EnableSensitiveDataLogging(true)
              .Options;
            _context = new MearskContext(options);
        }
        public decimal GetTotalOrderPrice(List<Orders> orders)
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
    }
}
