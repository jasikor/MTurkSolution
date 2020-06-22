using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MTurk.AI;
using MTurk.Algo;
using MTurk.Data;
using MTurk.DataAccess;
using MTurk.SQLDataAccess;
using System.IO;

namespace MTurk
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddTransient<IHistoricalGamesService, HistoricalGamesService>();
            services.AddTransient<IMoveEngine, NearestNeighbourMoveEngine>();
            services.AddTransient<ITrainingDataLoader, TrainingDataLoader>(); 
            services.AddTransient<INetworkStorage, DiskNetworkStorage>();
            services.AddSingleton<IAIManager, AIManager>();
            services.AddTransient<ISqlDataAccess, SqlDataAccess >();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<IGameParametersService, GameParametersService>();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "dwn");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
