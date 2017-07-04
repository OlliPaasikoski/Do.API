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
using Microsoft.AspNetCore.Diagnostics;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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

            // IUrlHelper required action context accessor to resolve action urls
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory => 
            {
                var actionContext =
                implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            DoContext doContext, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug(LogLevel.Information); // default level
            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            loggerFactory.AddNLog();

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
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            // debug logger for development
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
                        }

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

                cfg.CreateMap<Models.TaskForUpdateDto, Entities.Task>();
                cfg.CreateMap<Entities.Task, Models.TaskForUpdateDto>();

                cfg.CreateMap<Entities.BlogPost, Models.BlogPostDto>();
            });

            doContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
