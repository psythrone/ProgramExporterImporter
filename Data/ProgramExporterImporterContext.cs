using Microsoft.EntityFrameworkCore;
using ProgramExporterImporter.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProgramExporterImporter.Data
{
    class ProgramExporterImporterContext : DbContext
    {
        public DbSet<Product> Products { get; protected set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=OrderManager;Trusted_Connection=True");
        }
    }
}
