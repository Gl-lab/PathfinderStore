using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Services;
using Pathfinder.Core.Entities;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Core.Repositories;
using Pathfinder.Core.Repositories.Base;
using Pathfinder.Infrastructure.Repository;
using Pathfinder.Infrastructure.Repository.Base;
using System;
using AutoMapper;
using Pathfinder.Application.Mapper;

namespace Pathfinder.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PgDbContext>(options => options.UseNpgsql(Configuration["Data:WebDB:ConnectionString"]));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration["Data:WebDB:ConnectionString"]));

           
           services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            /*
            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();
                */
            services.AddControllersWithViews();
            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pathfinde API", Version = "v1" });
            });
            services.AddControllersWithViews();
            services.AddRazorPages();

            /* services.AddSpaStaticFiles(configuration =>
             {
                 configuration.RootPath = "ClientApp/dist";
             });*/
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddAutoMapper(typeof(CategoryProfile),typeof(ProductProfile));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
           /* if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }*/
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pathfinder API V1");
            });

            app.UseRouting();
           /* app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
           

           /* app.UseSpa(spa =>
            {
                
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });*/
            //IdentitySeedData.EnsurePopulated(serviceProvider);
            PathfinderSeed.SeedAsync(serviceProvider).Wait();
        }
    }
}
