using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using MediatR;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;

using Banking.Api.Extensions;
using Banking.Persistence.EFCore;
using AutoMapper;

namespace Banking.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseSqlite(
                       Configuration.GetConnectionString("ApplicationConnection"),
                       b => b.MigrationsAssembly(typeof(Startup).Assembly.FullName) //keep migratins in the api for simplicity
                   )
                );


            //Confgure MediatR for sending in-process command/queries (CQRS)
            services.AddMediatR(typeof(Startup));

            //Configure AutoMapper for models to and from commands/queries/entities
            services.AddAutoMapper(typeof(Startup));

            //Use fluent validation in place of default asp.net annotations
            services.AddControllers().AddFluentValidation(conf =>
                conf.RegisterValidatorsFromAssemblyContaining<Startup>()
            );

            //we'll use microsoft's versioning library for now
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
            });

            ///see https://github.com/khellang/Middleware
            services.AddProblemDetails();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseProblemDetails();

            if (env.IsDevelopment())
            {
                app.EnsureDatabaseSeeded("database.json");
            }
            
            if(env.IsStaging() || env.IsProduction())
            {
                app.UseHsts();
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
