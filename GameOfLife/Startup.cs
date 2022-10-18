using GameOfLife;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace RadarDataVisualizer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>()
                .AddTransient<LifeWindow>();
        }
    }
}
