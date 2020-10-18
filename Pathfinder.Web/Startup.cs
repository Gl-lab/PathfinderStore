using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices;
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
using VueCliMiddleware;
using Pathfinder.Web.fromNucleus.Extensions;
using Microsoft.AspNetCore.Http;
using Pathfinder.Application.Interfaces.Auth;

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
                options.UseNpgsql(Configuration["Data:WebDB:ConnectionString"]).UseLazyLoadingProxies());

            services.ConfigureAuthentication();
            services.ConfigureJwtTokenAuth(Configuration);
            services.ConfigureCors(Configuration);
            services.ConfigureDependencyInjection();
            // services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //     .AddEntityFrameworkStores<ApplicationDbContext>();
                
            // services.AddIdentityServer()
            //     .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            // services.AddAuthentication()
            //     .AddIdentityServerJwt();
                
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pathfinde API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
            });
            //services.AddRazorPages();

            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IUserService, UserService>();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSpaStaticFiles();
           /* if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }*/
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pathfinder API V1"));
            app.UseCors(Configuration["App:CorsOriginPolicyName"]);
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                    spa.Options.SourcePath = "ClientApp";
                else
                    spa.Options.SourcePath = "dist";

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve");
                }
            });
            //IdentitySeedData.EnsurePopulated(serviceProvider);
            PathfinderSeed.SeedAsync(serviceProvider).Wait();
        }
    }
}
