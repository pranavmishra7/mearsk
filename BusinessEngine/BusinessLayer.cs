using Data;
using PromotionEngine;
using System;
using System.Collections.Generic;

namespace BusinessEngine
{
    public class BusinessLayer
    {
        public  decimal GetTotalOrderPrice(List<Orders> orders)
        {
            PromoCalculator promoCalculator = new PromoCalculator();
            return promoCalculator.GetTotalOrderPrice(orders);
        }
        public string GenerateSliptForPayment(Payment payment)
        {

          return  GetPaymentSliptType(payment.ProductDetail.ProductType, payment.ProductDetail.ProductName);
        }
        string GetPaymentSliptType(string productType, string productName)
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
    }
}
