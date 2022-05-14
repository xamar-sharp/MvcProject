using System;
using Xunit;
using System.Linq;
using MvcProject;
using MvcProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;
namespace MvcProject.Tests
{
    public class DbContextTests
    {
        [Fact]
        public void CanConnect()
        {
            DbContextOptionsBuilder<WebContext> builder = new();
            builder.UseSqlServer(SecureData.ConnectionString);
            WebContext ctx = new WebContext(builder.Options);
            Assert.True(ctx.Database.CanConnect());
        }
        [Fact]
        public void CanLog()
        {
            Logger logger = new Logger("C:\\log.json");
            logger.Log(LogLevel.Information, "test log");
        }
        [Fact]
        public void CanAuthorize()
        {
            Mock<SpaceAuthorizationHandler> mock = new Mock<SpaceAuthorizationHandler>();
            AuthorizationHandlerContext ctx = new AuthorizationHandlerContext(new[] { new AuthorizationRequirement() }, null, null);
            SpaceAuthorizationHandler handler = mock.Object;
            handler.HandleAsync(ctx).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.NotNull(handler);
        }
        [Fact]
        public void CanReadSecureData()
        {
            Assert.NotSame(SecureData.ConnectionString, null);
            Assert.NotNull(SecureData.AdminPassword);
        }
        [Fact]
        public void CanCrud()
        {
            DbContextOptionsBuilder<WebContext> builder = new();
            builder.UseSqlServer(SecureData.ConnectionString);
            WebContext ctx = new WebContext(builder.Options);
            ctx.Users.Add(new User() { Email = "string@gmail.com", PasswordHash = SecureData.HashPassword("hello"), IsAlive = true });
            ctx.SaveChanges();
            Assert.NotEmpty(ctx.Users.ToListAsync().ConfigureAwait(false).GetAwaiter().GetResult());
            ctx.Users.First(user => user.Email == "string@gmail.com").IsAlive = false;
            ctx.SaveChanges();
        }
    }
}
