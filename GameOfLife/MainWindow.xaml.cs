using System;
using System.Collections.Generic;
using System.Windows;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Window> windows = new List<Window>();

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new LifeWindow(this.checkBox.IsChecked.Value);
            this.windows.Add(window);
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.windows.ForEach(w => w.Close());
            this.windows.Clear();
        }
    }
}
