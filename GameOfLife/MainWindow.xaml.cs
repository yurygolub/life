using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private List<Window> windows = new List<Window>();

        public MainWindow(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = this.serviceProvider.GetRequiredService<LifeWindow>();
            this.windows.Add(window);
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.windows.ForEach(w => w.Close());
        }
    }
}
