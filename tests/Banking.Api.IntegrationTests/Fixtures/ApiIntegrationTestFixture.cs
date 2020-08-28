using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Banking.Persistence.EFCore;
using Castle.DynamicProxy.Contributors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using api = Banking.Api;

[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]

namespace Bank.Api.IntegrationTests
{
    public class ApiIntegrationTestFixture : IDisposable
    {
        public readonly IHost Server; //TestServer Server;
        public readonly HttpClient Client;
        public readonly IServiceProvider ServiceProvider;
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonFileName">File name of the json file containg the db fixture data</param>
        public ApiIntegrationTestFixture(string jsonFileName)
        {
            var hostBuilder = Host.CreateDefaultBuilder()
             .ConfigureLogging(logging =>
             {
                 //logging.ClearProviders(); // Remove other loggers
              })
             .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseTestServer();
                  webBuilder.UseStartup<api.Startup>();
                  webBuilder.UseEnvironment("IntegrationTest");
              });

            this.Server = hostBuilder.Start();
            this.Client = this.Server.GetTestClient();
            this.ServiceProvider = this.Server.Services;
            
            api.Extensions.JsonSeeder.EnsureDatabaseSeeded(this.ServiceProvider, jsonFileName, dropDatabase:true);
        }

        public virtual void Dispose()
        {
            this.Server.Dispose();
            this.Client.Dispose();
        }


    }
}
