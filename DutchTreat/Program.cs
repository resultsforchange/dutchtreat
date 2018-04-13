using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DutchTreat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();   // builds a webhost and starts listening for web requests
        }

        /// <summary>
        /// Creates a default builder for our WebHost and 
        /// tells it what class to use to setup how to listen 
        /// for web requests. It thens builds it, so that it 
        /// can run it.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(SetupConfiguration)
                .UseStartup<Startup>()
                .Build();

        private static void SetupConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder builder)
        {
            // removes the default builder options
            builder.Sources.Clear();
            // we're telling the system that we're going to need a file 
            // called config.json.
            builder.AddJsonFile("config.json", false, true)
                .AddXmlFile("config.xml", true)
                .AddEnvironmentVariables();
        }
    }
}
