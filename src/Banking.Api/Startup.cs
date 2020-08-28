using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using AutoMapper;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using MediatR;

using Banking.Api.Extensions;
using Banking.Persistence.EFCore;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore.Query.Internal;

////default conventions https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/conventions?view=aspnetcore-3.1
[assembly: ApiConventionType(typeof(DefaultApiConventions))]


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
            AddVersioningWithSwagger(services);

            ///see https://github.com/khellang/Middleware
            services.AddProblemDetails();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
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

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = string.Empty;
                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }


        private void AddVersioningWithSwagger(IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddVersionedApiExplorer(options =>
            {
                // format code will format the version as "'v'major[.minor][-status]"
                // versioning by url segment. the SubstitutionFormat can also be used to control the format of the API version in route templates
                //https://github.com/microsoft/aspnet-api-versioning/blob/master/samples/aspnetcore/SwaggerSample/Startup.cs
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Version = "v1", Title = "v1 Banking API" });
                options.CustomSchemaIds(x => x.FullName);//need this because we name everythiing Command/Query
            });

        }

     
    }
}
