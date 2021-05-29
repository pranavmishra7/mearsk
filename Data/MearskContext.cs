using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class MearskContext : DbContext
    {
        public MearskContext(DbContextOptions<MearskContext> options)
            : base(options)
        {
        }

        public DbSet<SKUList> SkuList { get; set; }
        public DbSet<ActivePromotions> ActivePromotions { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}
