using App.Data.Invoices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Invoices
{
    public class InvoicesContext : DbContext
    {
        public InvoicesContext(DbContextOptions<InvoicesContext> options) : base(options)
        {
        }
        public DbSet<InvoiceDTO> Invoices { get; set; }
        public DbSet<EntryDTO> Entries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceDTO>().ToTable("Invoices").HasKey(s => s.InvoiceID);
            modelBuilder.Entity<EntryDTO>().ToTable("InvoiceEntries").HasKey(s => s.EntryId);
        }
    }
}
