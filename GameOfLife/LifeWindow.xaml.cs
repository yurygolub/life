using System;
using System.Diagnostics;
using System.Windows;
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
        private readonly Stopwatch stopwatch = new Stopwatch();

        private WriteableBitmap writeableBitmap;

        private GameEngine gameEngine;

        private int rows;
        private int cols;

        public LifeWindow()
        {
            this.InitializeComponent();
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += this.CompositionTarget_Rendering;

            this.Closing += (o, e) =>
                CompositionTarget.Rendering -= this.CompositionTarget_Rendering;

            this.cols = (int)this.image.Width;
            this.rows = (int)this.image.Height;

            this.gameEngine = new GameEngine(this.rows, this.cols, 2);

            this.writeableBitmap = new WriteableBitmap(
                (int)this.image.Width,
                (int)this.image.Height,
                96,
                96,
                PixelFormats.Bgr24,
                null);

            this.image.Source = this.writeableBitmap;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            byte[][] field = this.gameEngine.GetCurrentField();

            try
            {
                this.writeableBitmap.Lock();

                unsafe
                {
                    for (int i = 0; i < this.rows; i++)
                    {
                        for (int j = 0; j < this.cols; j++)
                        {
                            IntPtr pBackBuffer = this.writeableBitmap.BackBuffer;

                            pBackBuffer += i * this.writeableBitmap.BackBufferStride;
                            pBackBuffer += j * 3;

                            int color_data = (byte)(field[i + 1][j + 1] * 255) << 16; // R

                            *(int*)pBackBuffer = color_data;
                        }
                    }
                }

                this.writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, this.cols, this.rows));
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
