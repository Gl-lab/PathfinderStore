using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using Microsoft.AspNetCore.Hosting;
using Autofac;
using System.Reflection;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Infrastructure.Repository.Auth;

namespace Pathfinder.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<>(options => options.UseNpgsql(Configuration["Data:WebDB:ConnectionString"]));
            services.AddDbContext<PgDbContext>(options =>
                options
                    .UseNpgsql(Configuration["Data:WebDB:ConnectionString"])
                    .UseLazyLoadingProxies());

            services.ConfigureAuthentication();
            services.ConfigureJwtTokenAuth(Configuration);
            services.ConfigureCors(Configuration);
            services.ConfigureDependencyInjection();

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

            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<ICharacterRepository, CharacterRepository>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IPermissionsRepository, PermissionsRepository>();
            services.AddScoped<IRolePermissionsRepository, RolePermissionsRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSpaStaticFiles();

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
            PathfinderSeed.SeedAsync(serviceProvider).Wait();
        }
    }
}