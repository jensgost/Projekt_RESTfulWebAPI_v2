using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Projekt_RESTfulWebAPI.ApiKey;
using Projekt_RESTfulWebAPI.Data;
using Projekt_RESTfulWebAPI.Models;
using Microsoft.AspNetCore.Authentication;
using System.IO;
using Microsoft.AspNetCore.Identity;

namespace Projekt_RESTfulWebAPI
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

            services.AddControllers();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Projekt_RESTfulWebAPI", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Projekt_RESTfulWebAPI", Version = "v2" });

                var docsPath = Path.Combine(AppContext.BaseDirectory, "Documentation.xml");
                c.IncludeXmlComments("Documentation.xml");

            });


            services.AddDbContext<Data.OurDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("OurDbContextConnection")));

            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<OurDbContext>();

            services.AddAuthentication("ApiTokenScheme")
                .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("ApiTokenScheme", null);
        }   

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Projekt_RESTfulWebAPI v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Projekt_RESTfulWebAPI v2");
                });

            }

            app.UseCors(options => options
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader()
                );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
