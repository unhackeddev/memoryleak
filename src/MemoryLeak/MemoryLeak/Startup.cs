using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MemoryLeak
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
            services.AddRazorPages();
            services.AddControllers();

            var appInsigths = Configuration.GetConnectionString("app");

            var applicationInsightsServiceOptions = new ApplicationInsightsServiceOptions();
            applicationInsightsServiceOptions.EnableEventCounterCollectionModule = true;
            applicationInsightsServiceOptions.ConnectionString = appInsigths;
            services.AddApplicationInsightsTelemetry(applicationInsightsServiceOptions);

            services.ConfigureTelemetryModule<EventCounterCollectionModule>((module, o) => {
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "time-in-gc"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "cpu-usage"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-0-size"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-1-size"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-2-size"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gc-heap-size"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-0-gc-count"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-1-gc-count"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-2-gc-count"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "loh-size"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gc-fragmentation"));
                module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gc-fragmentation"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
