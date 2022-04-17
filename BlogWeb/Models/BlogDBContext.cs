using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlogWeb.Models
{
    public partial class BlogDBContext : DbContext
    {
        public BlogDBContext()
        {
        }

        public BlogDBContext(DbContextOptions<BlogDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdminAccount> AdminAccounts { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Favorite> Favorites { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<UserAccount> UserAccounts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-B10V49V\\SQLEXPRESS; Database=BlogDB; Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Vietnamese_CI_AS");

            modelBuilder.Entity<AdminAccount>(entity =>
            {
                entity.ToTable("AdminAccount");

                entity.Property(e => e.AdminAccountId).HasColumnName("AdminAccountID");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AdminAccounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_AdminAccount_Roles");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CatId)
                    .HasName("PK_Category");

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.CatName).HasMaxLength(255);

                entity.Property(e => e.Description).HasMaxLength(255);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.Property(e => e.CommentId).HasColumnName("CommentID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.Property(e => e.UserAccountId).HasColumnName("UserAccountID");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK_Comment_Posts");

                entity.HasOne(d => d.UserAccount)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserAccountId)
                    .HasConstraintName("FK_Comment_UserAccount");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.ToTable("Favorite");

                entity.Property(e => e.FavoriteId).HasColumnName("FavoriteID");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.Property(e => e.UserAccountId).HasColumnName("UserAccountID");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK_Favorite_Posts");

                entity.HasOne(d => d.UserAccount)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.UserAccountId)
                    .HasConstraintName("FK_Favorite_UserAccount");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.Property(e => e.AdminAccountId).HasColumnName("AdminAccountID");

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.IsHot).HasColumnName("isHot");

                entity.Property(e => e.Thumb).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.AdminAccount)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.AdminAccountId)
                    .HasConstraintName("FK_Posts_AdminAccount");

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.CatId)
                    .HasConstraintName("FK_Posts_Categories");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleDescription).HasMaxLength(50);

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.ToTable("UserAccount");

                entity.Property(e => e.UserAccountId).HasColumnName("UserAccountID");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FirstName).HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Password).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
