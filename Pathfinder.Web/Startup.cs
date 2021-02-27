using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Pathfinder.Infrastructure.Data;
using System;
using VueCliMiddleware;
using Pathfinder.Web.fromNucleus.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Pathfinder.Application;

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
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    { 
                        new OpenApiSecurityScheme 
                        { 
                            Reference = new OpenApiReference 
                            { 
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" 
                            } 
                        },
                        Array.Empty<string>()
                    } 
                });
            });

            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");

            services.AddApplicationServices();
            services.AddInfrastructureServices();
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
                spa.Options.SourcePath = env.IsDevelopment() ? "ClientApp" : "dist";

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve");
                }
            });
            PathfinderSeed.SeedAsync(serviceProvider).Wait();
        }
    }
}