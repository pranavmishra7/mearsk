using BusinessEngine;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mearsk.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class MearskOrderController : ControllerBase
    {
        

        private readonly ILogger<MearskOrderController> _logger;
        private readonly BusinessLayer _businesslayer;
        public MearskOrderController(ILogger<MearskOrderController> logger)
        {
            _logger = logger;
            _businesslayer = new BusinessLayer();
        }

        [HttpGet]
        public IActionResult GetTotalOrderPrice()
        {
            List<Orders> singleOrderList = new List<Orders>();
            singleOrderList.Add(new Orders { SKU = "A", Quantity = 1 });
            singleOrderList.Add(new Orders { SKU = "B", Quantity = 1 });
            singleOrderList.Add(new Orders { SKU = "C", Quantity = 1 });

            return Ok(_businesslayer.GetTotalOrderPrice(singleOrderList));
        }
        [HttpGet]
        public IActionResult GetPaymentSlip()
        {
            Payment payment = new Payment
            {
                Amount = 100,
                ProductDetail = new Product
                {
                    ProductName = "Book",
                    ProductType = "Physical",
                    SkuCode = "A"
                }
            };
            return Ok(_businesslayer.GenerateSliptForPayment(payment));
        }
    }
}
