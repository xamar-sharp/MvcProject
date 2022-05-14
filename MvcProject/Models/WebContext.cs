using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Logging;
namespace MvcProject.Models
{
    public class WebContext:DbContext
    {
        private readonly ILogger _logger;
        public DbSet<User> Users { get; set; }
        public DbSet<FileReference> FileReferences { get; set; }
        public WebContext(DbContextOptions<WebContext> options) : base(options)
        {
            Database.EnsureCreated();
            var name = Environment.MachineName;
            _logger = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new LoggerProvider("C:\\log.json"));
            }).CreateLogger(this.GetType().FullName);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                builder.UseSqlServer(SecureData.ConnectionString).LogTo((message) =>
                {
                    _logger.LogInformation(Environment.TickCount, null, message);
                });
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new EntityTypeConfiguration());
            builder.Entity<FileReference>().HasOne(reference => reference.User).WithMany(user => user.References).HasForeignKey(reference => reference.UserId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
