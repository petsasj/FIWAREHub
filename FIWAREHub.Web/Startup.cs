using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using FIWAREHub.Models.Sql;
using FIWAREHub.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FIWAREHub.Web
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
            // Sets up thread safe datalayer all units of work
            var dictionary = PrepareDictionary();
            static XPDictionary PrepareDictionary()
            {
                var dict = new ReflectionDictionary();
                dict.GetDataStoreSchema(ConnectionHelper.GetPersistentTypes());
                return dict;
            }

            IDataStore store = XpoDefault.GetConnectionProvider(
                XpoDefault.GetConnectionPoolString(Configuration.GetConnectionString("DbConnectionString"), 5, 100),
                AutoCreateOption.DatabaseAndSchema);
            XpoDefault.DataLayer = new ThreadSafeDataLayer(dictionary, store);

            services.AddScoped<UnitOfWork>();
            services.AddSingleton<CachingService>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
