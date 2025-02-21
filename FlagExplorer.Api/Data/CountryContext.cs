using FlagExplorer.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FlagExplorer.Api.Data
{
    public class CountryContext : DbContext
    {
        public CountryContext(DbContextOptions<CountryContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }

        // Optional: You can seed data here in OnModelCreating if you like
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Country>().HasData(
            //    new Country { Id = 1, Name = "France", Flag = "🇫🇷", Population = 67022000, Capital = "Paris" },
            //    new Country { Id = 2, Name = "Brazil", Flag = "🇧🇷", Population = 212600000, Capital = "Brasília" },
            //    new Country { Id = 3, Name = "Japan", Flag = "🇯🇵", Population = 125800000, Capital = "Tokyo" }
            //    // Add more seeds as desired
            //);
        }
    }
}
