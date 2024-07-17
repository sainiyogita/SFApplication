using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForecastingApplication.Models
{
    public class SalesContext : DbContext
    {
        public SalesContext() : base("name=SalesContext")
        {
        }

        public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Return> Returns { get; set; }
}
}
