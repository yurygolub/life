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

        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly GameEngine gameEngine;
        private readonly WriteableBitmap writeableBitmap;

        public LifeWindow(bool fullScreenEnabled)
        {
            if (fullScreenEnabled)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
                this.Cursor = Cursors.None;
            }

            this.InitializeComponent();

            this.image.Height = Rows;
            this.image.Width = Cols;

            this.gameEngine = new GameEngine(Rows, Cols, 2);

            this.writeableBitmap = new WriteableBitmap(Cols, Rows, 96, 96, PixelFormats.Bgr24, null);

            this.image.Source = this.writeableBitmap;

            CompositionTarget.Rendering += this.CompositionTarget_Rendering;

            this.Closing += (o, e) =>
                CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
        }

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

                        int color_data = (byte)(field[i + 1][j + 1] * 255) << 16; // R

                        unsafe
                        {
                            *(int*)resultPtr = color_data;
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
