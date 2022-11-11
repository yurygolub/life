using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife
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
