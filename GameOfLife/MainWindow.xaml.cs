using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Window> windows = new ();

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new LifeWindowViewModel(this.checkBox.IsChecked.Value);
            var window = new LifeWindow(viewModel);
            this.windows.Add(window);
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.windows.ForEach(w => w.Close());
            this.windows.Clear();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
