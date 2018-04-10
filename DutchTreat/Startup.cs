using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DutchTreat
{
    public class Startup
    {
        /// <summary>
        /// <para>
        /// This method gets called by the runtime. Use this method to 
        /// add services to the container. For more information on how 
        /// to configure your application, 
        /// visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </para>
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
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
            app.UseDefaultFiles();
            // When the webserver comes up, we are going to tell it 
            // to add the service of serving static files as something 
            // it can do. Treats wwwroot as the root of the webserver.
            app.UseStaticFiles();       
        }
    }
}