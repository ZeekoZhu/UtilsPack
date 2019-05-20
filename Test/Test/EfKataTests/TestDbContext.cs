using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Test.EfKataTests
{
    public class BlogPost
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var blogPost = modelBuilder.Entity<BlogPost>();
            blogPost
                .ToTable("my_awesome_blog_posts")
                .HasKey(p => p.Id);
            blogPost
                .Property(p => p.Id).HasColumnName("my_awesome_post_id");
            blogPost.HasOne(p => p.Category).WithMany().HasForeignKey(p => p.CategoryId);
            blogPost.Property(p => p.CategoryId).HasColumnName("my_awesome_category_id");

            modelBuilder.Entity<Category>();
            base.OnModelCreating(modelBuilder);
        }
    }
}