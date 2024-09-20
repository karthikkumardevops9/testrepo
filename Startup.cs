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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NLog.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using MSRecordsEngine.Services;


namespace MsRecordEngine
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigurationItemFactory.Default.Targets.RegisterDefinition("BlobStorage", typeof(BlobStorageTarget));
            LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterServices();
            //httpclient registration;
            services.AddHttpClient<DocumentService>();
            //transit microservices registration;
            services.AddTransient<Microservices>();
            services.AddHttpContextAccessor();
            services.AddScoped(typeof (CommonControllersService<>));
            services.AddCors(options =>
            {
                //options.AddPolicy(name: "PolicyCore",
                //builder =>
                //{
                //    builder.WithOrigins(Configuration["WithOrigins"]);
                //    builder.WithMethods(Configuration["WithMethods"]);
                //    builder.WithHeaders(Configuration["WithHeaders"]);
                //});
            });
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog();
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "MS-RecordsEngine",
                    Version = "v1",
                    Description = "MS-Records Engine manager",
                    //TermsOfService = new Uri("https://rashik.com.np"),
                    //Contact = new OpenApiContact
                    //{
                    //    Name = "TAB FUSION",
                    //    Email = "info@tab.com",
                    //    Url = new Uri("https://tab.com"),
                    //},
                    //License = new OpenApiLicense
                    //{
                    //    Name = "Swagger Implementation License",
                    //    Url = new Uri("https://tab.com"),
                    //}

                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
               //c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Integrated MS-RecordsEngine v1");
                c.InjectStylesheet("/swagger-custom/swagger-custom-style.css");
                c.InjectJavascript("/swagger-custom/swagger-custom-script.js", "text/javascript");
            });

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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


