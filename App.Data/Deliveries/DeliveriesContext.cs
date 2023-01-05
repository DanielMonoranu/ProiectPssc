using App.Data.Deliveries.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Deliveries
{
    public class DeliveriesContext : DbContext
    {
        public DeliveriesContext(DbContextOptions<DeliveriesContext> options) : base(options)
        {
        }
        public DbSet<DeliveryDto> Deliveries { get; set; }
        public DbSet<EntryDto> Entries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeliveryDto>().ToTable("Deliveries").HasKey(s => s.DeliveryId);
            modelBuilder.Entity<EntryDto>().ToTable("DeliveryEntries").HasKey(s => s.EntryId);
        }
    }
}
