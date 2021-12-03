using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC_Crime_Start.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MVC_Crime_Start
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
            // Setup EF connection
          services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["Data:CrimeTrackerAzure:ConnectionString"]));



            // added from MVC template
            services.AddMvc();
            services.AddMvc(option => option.EnableEndpointRouting = false);
            //#######services.AddControllersWithViews();
        }



        // This is the version from the MVC template
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // This ensures that the database and tables are created as per the Models.
            //using (var servicescope = app.ApplicationServices.getservice<IServiceScopeFactory>().createscope())
            //{
            //    var context = servicescope.serviceprovider.getrequiredservice<applicationdbcontext>();
            //    context.database.ensurecreated();
            //}

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            }


            // https://stackoverflow.com/a/58072137/1385857
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            //#####app.UseHttpsRedirection();

            app.UseStaticFiles();

            //#### app.UseRouting();

            app.UseMvc(routes =>
                {
                    routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                });
        }

    }
}
