using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForecastingApplication.Models
{
    public class Return
    {
        public int ReturnID { get; set; }
        public int OrderID { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Reason { get; set; }

        public Order Order { get; set; }
        public decimal ReturnAmount { get; set; }
    }
}
