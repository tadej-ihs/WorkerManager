using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StopStartWorkers.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StopStartWorkers
{
    public class Startup
    {
        private static Dictionary<string, CancellationTokenSource> cancellationTokensList { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            cancellationTokensList = new Dictionary<string, CancellationTokenSource>();

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StopStartWorkers", Version = "v1" });

            });

            //services.AddSingleton<Worker1>().AddHostedService<Worker1>(p => p.GetRequiredService<Worker1>());
            //services.AddSingleton<Worker2>().AddHostedService<Worker2>(p => p.GetRequiredService<Worker2>());
            services.AddHostedService<Worker1>();
            services.AddHostedService<Worker2>();
         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StopStartWorkers v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }


        //public static void AddWorkerCancelledToken(string workerName, CancellationToken cancellationToken)
        //{
        //    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //    cancellationTokensList.Add(workerName, cancellationToken);
        //}

        //public static void GetWorkersCancelledTokens()
        //{
        //    var result = new List<object>();
        //    foreach (var worker in cancellationTokensList)
        //    {
        //        result.Add(new { workerName = worker.Key, cancellationToken = worker.Value });
        //    }
        //}

        //public static void StopWorker(string workerName)
        //{
        //    CancellationToken token;
        //    if (cancellationTokensList.TryGetValue(workerName, out token))
        //    {
        //        EdiWorker.
        //    }
        //}
    }
}
