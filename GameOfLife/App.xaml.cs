using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        protected override async void OnStartup(StartupEventArgs e)
        {
            await this.AppHost.StartAsync();

            var startupWindow = this.AppHost.Services.GetRequiredService<MainWindow>();
            startupWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await this.AppHost.StopAsync();
            this.AppHost.Dispose();

            base.OnExit(e);
        }
    }
}
