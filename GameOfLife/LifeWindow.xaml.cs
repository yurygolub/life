using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for LifeWindow.xaml.
    /// </summary>
    public partial class LifeWindow : Window
    {
        public LifeWindow(LifeWindowViewModel viewModel)
        {
            this.ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            this.InitializeComponent();

            CompositionTarget.Rendering += this.CompositionTarget_Rendering;
        }

        public LifeWindowViewModel ViewModel { get; }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this.ViewModel.DrawNextGeneration();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
        }
    }
}
