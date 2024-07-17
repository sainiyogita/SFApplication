using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForecastingApplication.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string State { get; set; }

        public Product Product { get; set; }
    }
}
