using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace MvcProject
{
    public class Startup
    {
        public Startup()
        {
            Configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).Add(new JsonConfigurationSource("configuration")).Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(opt =>
            {
                //opt.ModelBinderProviders.Insert(1, new ModelBinderProvider());
            }).AddViewLocalization().AddDataAnnotationsLocalization();
            services.Configure<MvcViewOptions>(opt =>
            {
                opt.ViewEngines.Insert(0, new HtmlViewEngine());
            });
            services.AddResponseCompression(opt =>
            {
                opt.EnableForHttps = true;
                opt.Providers.Add<CompositeCompressionProvider>();
            });
            services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromMinutes(10);
                opt.Cookie.Name = ".AspNetCore.Session";
                opt.Cookie.IsEssential = true;
                opt.Cookie.HttpOnly = false;
            });
            services.AddLocalization(opt => opt.ResourcesPath = "Resources");
            services.AddCors(opt => opt.AddPolicy("default", builder => builder.AllowAnyOrigin().WithMethods("GET").AllowAnyHeader().AllowCredentials()));
            services.AddAuthentication("Cookies").AddCookie(opt =>
            {
                opt.Cookie.HttpOnly = false;
                opt.Cookie.IsEssential = true;
                opt.Cookie.MaxAge = TimeSpan.FromMinutes(10);
                opt.LoginPath = new PathString("/Authorization/Login");
                opt.AccessDeniedPath = new PathString("/Authorization/Login");
            });
            //services.AddSingleton<IAuthorizationHandler,SpaceAuthorizationHandler>();
            ILogger _logger = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new LoggerProvider("C:\\log.json"));
            }).CreateLogger(this.GetType().FullName);
            services.AddDbContext<MvcProject.Models.WebContext>(opt => opt.UseSqlServer(SecureData.ConnectionString).LogTo((message) =>
              {
                  _logger.LogInformation(Environment.TickCount, null, message);
              }));
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("default", opt =>
                {
                    opt.Requirements.Add(new AuthorizationRequirement());
                });
            });
            services.AddDistributedMemoryCache();
            services.AddHsts(opt =>
            {
                opt.IncludeSubDomains = true;
                opt.MaxAge = TimeSpan.FromDays(1);
                opt.Preload = true;
            });
            services.AddHttpsRedirection(opt =>
            {
                opt.RedirectStatusCode = 301;
            });
            services.Configure<ConfigurationObject>(Configuration);
            services.Configure<RouteOptions>(opt => opt.ConstraintMap.Add("secret", typeof(CookieRouteConstraint)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseFileServer(new FileServerOptions()
            {
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = true,
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Environment.CurrentDirectory),
                RequestPath = new Microsoft.AspNetCore.Http.PathString("/secure/data")
            });
            app.UseStatusCodePagesWithReExecute("/secure/error", "?state={0}");
            app.UseRequestLocalization(new RequestLocalizationOptions()
            {
                SupportedCultures = new CultureInfo[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ru-RU")
                },
                SupportedUICultures = new CultureInfo[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ru-RU")
                },
                DefaultRequestCulture = new RequestCulture("en-US")
            });
            app.UseResponseCompression();
            //app.UseMiddleware<TrafficMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(opt =>
            {
                opt.MapAreaControllerRoute("default", "Standard", "{controller=Home}/{action=Index}/{id?}", new { }, new { id = new IntRouteConstraint() });
            });
        }
    }
    public struct TrafficMiddleware
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _del;
        public TrafficMiddleware(Microsoft.AspNetCore.Http.RequestDelegate del)
        {
            _del = del;
        }
        public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext ctx)
        {
            if (ctx.Request.IsHttps && ctx.Connection.RemoteIpAddress != System.Net.IPAddress.None && ctx.Request.Query.ContainsKey("token") && ctx.GetServerVariable("SERVER_PORT_SECURE") == "1")
            {
                ctx.Response.ContentType = "application/xml";
                await ctx.Response.SendFileAsync(new FileInfo("C:\\obj.xml"));
            }
            else
            {
                await _del.Invoke(ctx);
            }
        }
    }
    public class CookieRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext ctx, IRouter router, string value, RouteValueDictionary dictionary, RouteDirection direction)
        {
            if (dictionary.ContainsKey(value) && dictionary[value] is object routeValue && ctx.Items.ContainsKey(value) is object itemValue && ctx.Session.Keys.Contains(value) && ctx.Session.GetString(value) is string sessionValue)
            {
                return routeValue == itemValue && sessionValue == "Confirmed";
            }
            return true;
        }
    }
    public struct Entity
    {
        public string Name { get => "Anton"; }
    }
    public class TemplateRouter : IRouter
    {
        public async Task RouteAsync(RouteContext ctx)
        {
            ctx.Handler = ctx.HttpContext.Request.Form.ContainsKey("id") && ctx.HttpContext.Request.Query.ContainsKey("id") && ctx.HttpContext.Request.ContentLength >= 0 ? async delegate (HttpContext ctx)
             {
                 await ctx.Response.WriteAsJsonAsync(new Entity());
             }
            : async ctx => { };
            await Task.FromResult(0);
        }
        public VirtualPathData GetVirtualPath(VirtualPathContext ctx)
        {
            throw new NotImplementedException();
        }
    }
    public class FileInfo : Microsoft.Extensions.FileProviders.IFileInfo
    {
        public DateTimeOffset LastModified { get => System.IO.File.GetLastWriteTime(PhysicalPath); }
        public string Name { get => System.IO.Path.GetFileName(PhysicalPath); }
        public string PhysicalPath { get; set; }
        public bool IsDirectory { get => System.IO.File.GetAttributes(PhysicalPath) == System.IO.FileAttributes.Directory; }
        public long Length { get => System.IO.File.ReadAllBytes(PhysicalPath).Length; }
        public bool Exists { get => System.IO.File.Exists(PhysicalPath); }
        public FileInfo(string physicalPath)
        {
            PhysicalPath = physicalPath;
        }
        public System.IO.Stream CreateReadStream()
        {
            return System.IO.File.OpenRead(PhysicalPath);
        }
    }
}
