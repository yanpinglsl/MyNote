using System;
using System.Collections.Generic;
using System.Text;

namespace YY.MicroService.Model
{
   public class Order
    {
        public long OrderId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Amount { get; set; }
    }
}
