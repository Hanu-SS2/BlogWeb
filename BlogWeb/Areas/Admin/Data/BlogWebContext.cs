#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Areas.Admin.Models;

namespace BlogWeb.Data
{
    public class BlogWebContext : DbContext
    {
        public BlogWebContext (DbContextOptions<BlogWebContext> options)
            : base(options)
        {
        }

        public DbSet<BlogWeb.Areas.Admin.Models.Post> Post { get; set; }
    }
}
