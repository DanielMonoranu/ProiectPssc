using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace App.Data
{
    public class OrdersContext: DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options) : base(options) { 
        
        }

        public DbSet<OrderDTO> Orders { get; set; }
        public DbSet<OrderRegistrationDTO> OrderRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<OrderDTO>().ToTable("Order").HasKey(s => s.OrderId);
            modelBuilder.Entity<OrderRegistrationDTO>().ToTable("OrderRegistration").HasKey(s => s.EntryId);
        }
    }
}
