using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LifeGameService;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for LifeWindow.xaml.
    /// </summary>
    public partial class LifeWindow : Window
    {
        private const int Rows = 1080;
        private const int Cols = 1920;

        private readonly Stopwatch stopwatch = new ();
        private readonly GameEngine gameEngine = new (Rows, Cols, 2);
        private readonly WriteableBitmap writeableBitmap = new (Cols, Rows, 96, 96, PixelFormats.Bgr24, null);

        private ActionCommand changeFullScreenMode;

        private bool isFullScreen;

        public LifeWindow(bool fullScreenEnabled)
        {
            if (fullScreenEnabled)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.Cursor = Cursors.None;
                this.isFullScreen = true;
            }

            this.InitializeComponent();

            this.image.Height = Rows;
            this.image.Width = Cols;

            this.image.Source = this.writeableBitmap;

            CompositionTarget.Rendering += this.CompositionTarget_Rendering;

            this.Closing += (o, e) =>
                CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
        }

        public ICommand ChangeFullScreenMode => this.changeFullScreenMode ??= new ActionCommand(() =>
        {
            this.isFullScreen = !this.isFullScreen;
            if (this.isFullScreen)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.Cursor = Cursors.None;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.ResizeMode = ResizeMode.CanResize;
                this.Cursor = Cursors.Arrow;
            }
        });

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this.DrawNextGeneration();
        }

        private void DrawNextGeneration()
        {
            byte[][] field = this.gameEngine.GetCurrentField();

            try
            {
                this.writeableBitmap.Lock();

                IntPtr backBufferPtr = this.writeableBitmap.BackBuffer;
                int stride = this.writeableBitmap.BackBufferStride;

                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        IntPtr resultPtr = backBufferPtr;

                        resultPtr += i * stride;
                        resultPtr += j * 3;

                        int colorData = (field[i + 1][j + 1] * 255) << 16; // R

                        unsafe
                        {
                            *(int*)resultPtr = colorData;
                        }
                    }
                }

                this.writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, Cols, Rows));
            }
            finally
            {
                this.writeableBitmap.Unlock();
            }

            this.gameEngine.NextGeneration();

            this.stopwatch.Stop();

            this.Title = $"Frame time: {this.stopwatch.ElapsedMilliseconds} ms";

            this.stopwatch.Restart();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
