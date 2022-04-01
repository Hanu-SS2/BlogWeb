using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Models;

namespace BlogWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BlogWeb.Models.Category> Category { get; set; }
        public DbSet<BlogWeb.Models.Account> Account { get; set; }
        public DbSet<BlogWeb.Models.Post> Post { get; set; }
        public DbSet<BlogWeb.Models.Role> Role { get; set; }
    }

   
}