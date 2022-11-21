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
        private readonly IServiceProvider serviceProvider;

        public MainWindow(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.serviceProvider.GetRequiredService<LifeWindow>().Show();
        }
    }
}
