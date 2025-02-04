using DinoPizza.Authorize;
using DinoPizza.BusinessLogic;
using DinoPizza.Domains;
using DinoPizza.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DinoPizza.DataAccessLayer
{
    public class DinoDBContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public DbSet<ClientModel> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public DinoDBContext(DbContextOptions options) : base(options)
        {
        }
    }
}