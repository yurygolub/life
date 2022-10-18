using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.GetRequiredService<LifeWindow>().Show();
        }
    }
}
