using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreServices.Configuration;
using CoreServices.Filter;
using CoreServices.Interface;
using CoreServices.Log4Net;
using CoreServices.Middleware;
using CoreServices.Models;
using CoreServices.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace CoreServices
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Post APIs", Version = "V1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddCors(option => option.AddPolicy("MyBlogPolicy", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

            //Use exceptions to modify the response
            services.AddMvc(option => option.Filters.Add(new HttpResponseExceptionFilter())).SetCompatibilityVersion(CompatibilityVersion.Version_2_1); 

            services.AddDbContext<BlogDBContext>(item => item.UseSqlServer(Configuration.GetConnectionString("BlogDBConnection")));
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IClientConfiguration, ClientConfiguration>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
                // app.UseDeveloperExceptionPage();
                Console.WriteLine(env.EnvironmentName);
            }
            else if (env.IsProduction() || env.IsStaging())
            {
                Console.WriteLine(env.EnvironmentName);
                app.UseExceptionHandler("/error");
            }
            else
            {
                app.UseHsts();
            }


            //logger.LogInformation("Hello", "Hello");
            log4net.LogManager.GetLogger(typeof(Startup)).Info("Startup Invoked");

            loggerFactory.AddLog4Net();

            app.UseMiddleware<RequestLoggerMiddleware>();
            app.UseClientConfigurationMiddleware();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Post APIs");
            });

            app.UseHttpsRedirection();
            app.UseCors("MyBlogPolicy");
            app.UseMvc();
        }
    }
}
