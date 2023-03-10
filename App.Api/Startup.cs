using App.Data;
using App.Data.Deliveries;
using App.Data.Deliveries.Repositories;
using App.Data.Invoices;
using App.Data.Invoices.Repositories;
using App.Data.Repositories;
using App.Domain;
using App.Domain.Deliveries;
using App.Domain.Deliveries.Repositories;
using App.Domain.Invoices;
using App.Domain.Invoices.Repositories;
using App.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api
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
            services.AddDbContext<OrdersContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IOrderRepository, OrdersRepository>();
            services.AddTransient<IOrderRegistrationNumberRepository, OrderRegistrationsRepository>();
            services.AddTransient<CancelOrderWorkflow>();


            services.AddDbContext<DeliveriesContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IDeliveriesRepository, DeliveriesRepository>();
            services.AddTransient<IEntriesRepository, EntriesRepository>();
            services.AddTransient<CancelDeliveryWorkflow>();

            services.AddDbContext<InvoicesContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IInvoicesRepository, InvoicesRepository>();
            services.AddTransient<IInvoiceEntriesRepository, InvoiceEntriesRepository>();
            services.AddTransient<CancelInvoiceWorkflow>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "App.Api", Version = "v1" });
            });

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "App.Api v1"));
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
