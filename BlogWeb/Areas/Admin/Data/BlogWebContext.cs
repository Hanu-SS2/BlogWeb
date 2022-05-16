using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Models;

namespace BlogWeb.Data
{
    public class BlogWebContext : DbContext
    {
        public BlogWebContext (DbContextOptions<BlogWebContext> options)
            : base(options)
        {
        }

        public DbSet<BlogWeb.Models.AdminAccount>? AdminAccount { get; set; }

        public DbSet<BlogWeb.Models.Category>? Category { get; set; }

        public DbSet<BlogWeb.Models.Comment>? Comment { get; set; }

        public DbSet<BlogWeb.Models.Favorite>? Favorite { get; set; }

        public DbSet<BlogWeb.Models.Post>? Post { get; set; }

        public DbSet<BlogWeb.Models.Role>? Role { get; set; }

        public DbSet<BlogWeb.Models.UserAccount>? UserAccount { get; set; }
    }
}
