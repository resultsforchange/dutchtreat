using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DutchTreat.Services;
using DutchTreat.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AutoMapper;

namespace DutchTreat
{
    public class Startup
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// <para>
        /// Create a constructor because startup is going to allow us to 
        /// inject certain very basic interfaces that are actually set up 
        /// in the program.cs into our startup, and one of these we'll 
        /// probably want to use is IConfiguration.
        /// </para>
        /// <para>
        /// From config, we'll be able to go down to SQL Server and just with
        /// an array indexer give it a key to some value I want to store, but
        /// configuration actually has connection strings as a special property
        /// to get values, so we're going to go here and get it.
        /// </para>
        /// </summary>
        public Startup(IConfiguration config)
        {
            _config = config;
        }


        /// <summary>
        /// <para>
        /// This method gets called by the runtime. Use this method to 
        /// add services to the container. For more information on how 
        /// to configure your application, 
        /// visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </para>
        /// <para>
        ///     There are three types of services that you can add:
        ///     <list type="table">
            ///     <item>
            ///         <listheader>
            ///             <term>AddTransient</term>
            ///             <description>
            ///                 Don't have any data on themselves, often just methods 
            ///                 that do things
            ///             </description>
            ///         </listheader>
            ///     </item>
            ///     <item>
            ///         <listheader>
            ///             <term>AddScoped</term>
            ///             <description>
            ///                 Add services that are a little bit more expensive 
            ///                 to create, but they're kept around for the length of a connection. 
            ///                 They actually can be kept in different scopes, but the default 
            ///                 scope is the length of a request from a client. Scoped allows you 
            ///                 to cache instances of an object to be reused within this idea 
            ///                 called a scope. PRactically what this means, is for web applications,
            ///                 for each request a scope is begun and ended, so that you'll get the 
            ///                 same instance of the object that's marked as scoped through that single
            ///                 request, so if you have many layers that you are going through, 
            ///                 they are all going to be using the same object.
            ///             </description>
            ///         </listheader>
            ///     </item>
            ///     <item>
            ///         <listheader>
            ///             <term>AddSingleton</term>
            ///             <description>
            ///                 These are for services that are created once and are kept 
            ///                 for the lifetime of the server being up. 
            ///             </description>
            ///         </listheader>
            ///     </item>
        ///     </list>
        /// </para>
        /// </summary>
        /// <param name="services"></param>
        /// 
        public void ConfigureServices(IServiceCollection services)
        {
            // What we are saying here is, please make the DbContext part of the 
            // service collection, so that I can inject it into different services 
            // that I need, for example, inside of a controller.
            services.AddDbContext<DutchContext>(cfg =>
            {
                cfg.UseSqlServer(_config.GetConnectionString("DutchConnectionString"));
            });

            // Add support for AutoMapper
            services.AddAutoMapper();

            // The magic here is that the consumer of the mailservice shouldn't care 
            // whether we have a real or a fake one, it is just going to have an 
            // implementation of the interface that we can call.
            services.AddTransient<INullMailService, NullMailService>();

            // Add a service for DutchSeeder class.
            services.AddTransient<DutchSeeder>();

            // Register DutchTreat repository to the service layer so that we can use it.

            // We using AddScoped because we want the repository to be shared within one
            // complete scope, which is usually a request. In this way we are not constructing
            // them over and over again. We are actually going to reuse it through a scope,
            // and then it'll be deconstructed as necessary. 

            // The code below is saying, add IDutchRepository as a service that people can 
            // use, but use the implementation that we provided.
            services.AddScoped<IDutchRepository, DutchRepository>();

            // tell Json to ignore self referencing loops
            services.AddMvc()
                .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        /// <summary>
        /// <para>
        /// This method is essentially saying, when you start up, 
        /// tell me how you want to listen for web requests - What 
        /// do you want me to do when a web request is executed.
        /// </para>
        /// <para>
        /// This method gets called by the runtime. Use this method 
        /// to configure the HTTP request pipeline.
        /// </para>
        /// <para>
        /// By default, ASP.net does NOT serve files! We want to tell 
        /// it that we want to configure our webserver to do something.
        /// Remember that the order of the configuration matters. In this
        /// method we are configuring the order in which the middleware 
        /// runs. Middleware => when a request comes in, I want you to 
        /// run something for me. All the middleware knows, is once they 
        /// are done with their work, they need to call the next piece 
        /// of middleware.
        /// </para>
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Allows the webserver to serve default files (index.html, etc)
            // Looks for a blank directory URL, searches for index.html 
            // and deliver that file.
            // app.UseDefaultFiles();

            // We need to get rid of the line above as we are now not using the 
            // static index.html file, but mvc, i.e. we are not serving HTML 
            // anymore. If we leave this line, it will continue seving HTML and not
            // the mvc views.

            // This tells us that we want to support pages to be shown when
            // exceptions are thrown, not when status codes happen, like File 
            // Not Found or Unauthorized, but really an exception is thrown. 
            // Remember that this is for developer's use only. 
            // 
            // We can set up the system to only show this in a dev environment 
            // by doing the following:
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Shows an actual error page. This is going to be a catch-all
                // page that's going to show errors. This will only be displayed 
                // when we are not in a development environment. 
                app.UseExceptionHandler("/error");
            }
            // So how does the system know what type of environment it is 
            // running in? This set up in the project properties, in the Debug
            // tab. The environment variable ASPNETCORE_ENVIRONMENT is used to
            // indicate the environment that the app is running in. Change the 
            // value to the environment that the app is running in, to indicate 
            // how to deal with stuff.

            // When the webserver comes up, we are going to tell it 
            // to add the service of serving static files as something 
            // it can do. Treats wwwroot as the root of the webserver.
            app.UseStaticFiles();
            // We're going to listen to requests and see if we can map 
            // them to a controller which will then map them to a view 
            // for us.
            //
            // The code below creates a default route
            // for all requests that come in, parses the url 
            // string according to the pattern below and sees 
            // if it can map it to the corresponding controller
            // and action. If it can't map the request, it just 
            // maps to the controller and action below.
            app.UseMvc(cfg => {
                
                cfg.MapRoute("Default",
                    "{controller}/{action}/{id?}",
                    new { controller = "App", Action = "Index" });
            });

            // We don't wan to run the seed method when we are in production
            if (env.IsDevelopment())
            {
                // seed the database
                // we are creating a scope that only lasts for the life time 
                // of this particular piece of code
                using(var scope = app.ApplicationServices.CreateScope())
                {
                    // we are going to create a scope, create the object to 
                    // seed the database, and then we are going to make it 
                    // all go away

                    // we are asking for the scope to give us the ServiceProvider 
                    // (an object that can create instances of services)
                    // This is going to create an instance of the DutchSeeder 
                    // inside of our scope, along with any other required services.
                    var seeder = scope.ServiceProvider.GetService<DutchSeeder>();
                    seeder.Seed();
                }
            }
        }
    }
}