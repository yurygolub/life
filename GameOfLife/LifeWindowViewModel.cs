using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LifeGameService;

#pragma warning disable CS0067

namespace GameOfLife
{
    public class LifeWindowViewModel : INotifyPropertyChanged
    {
        private const int Rows = 1080;
        private const int Cols = 1920;

        private readonly Stopwatch stopwatch = new ();
        private readonly GameEngine gameEngine = new (Rows, Cols, 2);
        private readonly WriteableBitmap writeableBitmap = new (Cols, Rows, 96, 96, PixelFormats.Bgr24, null);

        private ICommand changeFullScreenMode;

        private bool isFullScreen;

        public LifeWindowViewModel(bool fullScreenEnabled)
        {
            if (fullScreenEnabled)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.Cursor = Cursors.None;
                this.isFullScreen = true;
            }

            this.ImageSource = this.writeableBitmap;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Height => Rows;

        public int Width => Cols;

        public ImageSource ImageSource { get; set; }

        public WindowState WindowState { get; set; }

        public WindowStyle WindowStyle { get; set; } = WindowStyle.SingleBorderWindow;

        public ResizeMode ResizeMode { get; set; } = ResizeMode.CanResize;

        public Cursor Cursor { get; set; }

        public string Title { get; set; }

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

        public void DrawNextGeneration()
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

                        int colorData = (field[i + 1][j + 1] * 255) << 16; // Red

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
    }
}
