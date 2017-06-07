using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Do.API.Entities;
using Do.API.Services;

namespace Do.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(setupActions =>
            {
                setupActions.ReturnHttpNotAcceptable = true;
                setupActions.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setupActions.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
            });

            var connectionString = Configuration["connectionStrings:doDBConnectionString"];
            services.AddDbContext<DoContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskCategoryRepository, TaskCategoryRepository>();
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            DoContext doContext, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async (context) =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.TaskCategory, Models.TaskCategoryDto>();
                cfg.CreateMap<Entities.Task, Models.TaskDto>();
                cfg.CreateMap<Models.TaskForCreationDto, Entities.Task>();
                cfg.CreateMap<Models.TaskCategoryForCreationDto, Entities.TaskCategory>();
            });

            doContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
