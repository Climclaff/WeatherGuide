using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherGuide.Models;
namespace WeatherGuide.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Clothing> Clothings { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Review> Reviews { get; set; }

    }
}
