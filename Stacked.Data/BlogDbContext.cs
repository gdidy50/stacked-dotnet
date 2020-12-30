using Microsoft.EntityFrameworkCore;
using Stacked.Data.Models;

namespace Stacked.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext() { }
        public BlogDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // An Article has an Author, who has many Articles
            modelBuilder.Entity<Article>()
                .HasOne(article => article.Author)
                .WithMany(author => author.Articles);

            // A Comment belongs to a single Article, which has many Comments
            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.Article)
                .WithMany(article => article.Comments)
                .HasForeignKey(comment => comment.ArticleId);

            // Articles <=> Tags is a Many <=> Many relationship
            // As such, there is a join table ArticleTag with a Composite Key
            modelBuilder.Entity<ArticleTag>()
                .HasKey(at => new { at.ArticleId, at.TagId });

            // Every ArticleTag relates to an Article
            modelBuilder.Entity<ArticleTag>()
                .HasOne(at => at.Article)
                .WithMany(a => a.ArticleTags)
                .HasForeignKey(at => at.ArticleId);

            // Every ArticleTag relates to a Tag
            modelBuilder.Entity<ArticleTag>()
                .HasOne(at => at.Tag)
                .WithMany(a => a.ArticleTags)
                .HasForeignKey(at => at.TagId);
        }
    }
}
