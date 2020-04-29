using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raunstrup.BusinessLogic.ServiceInterfaces;
using Raunstrup.BusinessLogic.Services;
using Raunstrup.DataAccess.Context;

namespace Raunstrup.Api
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
            services.AddControllers();
            //Try'n Erro
            //services.AddDbContext<RaunstrupContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("RaunstrupContext")));
            services.AddDbContext<RaunstrupContext>(options =>
                options.UseInMemoryDatabase(databaseName: "RaunstrupDBAPIInMemory"));

            //options.UseSqlite(Configuration.GetConnectionString("MvcMovieContext")));

            services.AddScoped<IItemService, ItemService>();
            //--------------------------------------------------
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
