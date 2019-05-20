using System;
using System.Linq;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SqlKata;
using SqlKata.Compilers;
using Xunit;
using ZeekoUtilsPack.EFKata;

namespace Test.EfKataTests
{
    public class NameMappingTests : IDisposable
    {
        private readonly EfKataContext _efKata;
        private readonly ServiceProvider _sp;
        private readonly EfKataEntityContext<BlogPost> _blogPost;
        private readonly EfKataEntityContext<Category> _category;
        private readonly SqliteConnection _conn;

        public NameMappingTests()
        {
            var services = new ServiceCollection();
            services.AddEfKata<TestDbContext>();
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();
            services.AddDbContext<TestDbContext>(builder => builder.UseSqlite(_conn));
            _sp = services.BuildServiceProvider();
            _efKata = _sp.GetRequiredService<EfKataContext>();
            _blogPost = _efKata.Entity<BlogPost>();
            _category = _efKata.Entity<Category>();
        }

        [Fact]
        public void TableNameTests()
        {
            _efKata.Table<BlogPost>().Should().Be("my_awesome_blog_posts");
            _efKata.Table<Category>().Should().Be(nameof(Category));
            _blogPost.Table.Should().Be("my_awesome_blog_posts");
            _category.Table.Should().Be(nameof(Category));
        }

        [Fact]
        public void ColumnNameTests()
        {
            _efKata.Column((BlogPost p) => p.Id).Should().Be("my_awesome_blog_posts.my_awesome_post_id");
            _efKata.Column((Category c) => c.Name).Should().Be(nameof(Category) + "." + nameof(Category.Name));
            _blogPost.Column(p => p.Id).Should().Be("my_awesome_blog_posts.my_awesome_post_id");
            _category.Column(p => p.Name).Should().Be(nameof(Category) + "." + nameof(Category.Name));
        }

        [Fact]
        public void ColumnsNameTests()
        {
            _efKata.Columns<BlogPost>().Should()
                .BeEquivalentTo("my_awesome_blog_posts.my_awesome_post_id",
                    "my_awesome_blog_posts.my_awesome_category_id", "my_awesome_blog_posts." + nameof(BlogPost.Title));
            _efKata.Columns<Category>().Should()
                .BeEquivalentTo(nameof(Category) + "." +nameof(Category.Name), nameof(Category) + "." +nameof(Category.Id));
        }

        [Fact]
        public void QueryTest()
        {
            var db = _sp.GetRequiredService<TestDbContext>();
            db.Database.EnsureCreated();
            db.AddRange(
                new Category { Name = "Foo" }, new Category { Name = "Bar" });
            db.SaveChanges();
            db.AddRange(
                new BlogPost
                {
                    CategoryId = 1,
                    Title = "Foo"
                },
                new BlogPost
                {
                    CategoryId = 1,
                    Title = "Bar"
                }
            );
            db.SaveChanges();
            var compiler = new SqliteCompiler();
            var query = new Query(_blogPost.Table).Where(_blogPost.Column(p => p.Title), "=", "Foo")
                .Select(_blogPost.Columns);
            var compileResult = compiler.Compile(query);
            var conn = db.Database.GetDbConnection();
            var result = conn.Query<BlogPost>(compileResult.Sql, compileResult.NamedBindings).ToList();
            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Foo");
        }

        public void Dispose()
        {
            _conn?.Dispose();
            _sp?.Dispose();
        }
    }
}