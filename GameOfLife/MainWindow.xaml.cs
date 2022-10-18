using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ServiceProvider.GetRequiredService<LifeWindow>().Show();
        }
    }
}
