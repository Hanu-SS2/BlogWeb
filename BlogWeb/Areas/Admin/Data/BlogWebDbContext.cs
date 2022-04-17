#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Areas.Admin.Models;

namespace BlogWeb.Data
{
    public class BlogWebDbContext : DbContext
    {
        public BlogWebDbContext (DbContextOptions<BlogWebDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogWeb.Areas.Admin.Models.Comment> Comment { get; set; }
    }
}
