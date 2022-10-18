using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RadarDataVisualizer;
using System.Windows;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.AppStartup = new Startup();

            this.AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices(s =>
                {
                    this.AppStartup.ConfigureServices(s);
                })
                .Build();
        }

        public IHost AppHost { get; }

        public Startup AppStartup { get; }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.AppHost.Start();

            var startupWindow = this.AppHost.Services.GetRequiredService<MainWindow>();
            startupWindow.Show();

            base.OnStartup(e);
        }
    }
}
