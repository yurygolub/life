using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for LifeWindow.xaml.
    /// </summary>
    public partial class LifeWindow : Window
    {
        private static WriteableBitmap writeableBitmap;

        private readonly Stopwatch stopwatch = new Stopwatch();

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
            this.Closing += (o, e) => CompositionTarget.Rendering -= this.CompositionTarget_Rendering;

            this.cols = (int)this.image.Width;
            this.rows = (int)this.image.Height;

            this.gameEngine = new GameEngine(this.rows, this.cols, 2);

            writeableBitmap = new WriteableBitmap(
                (int)this.image.Width,
                (int)this.image.Height,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            this.image.Source = writeableBitmap;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this.stopwatch.Restart();

            byte[][] field = this.gameEngine.GetCurrentGeneration();

            try
            {
                // Reserve the back buffer for updates.
                writeableBitmap.Lock();

                unsafe
                {
                    for (int i = 0; i < this.rows; i++)
                    {
                        for (int j = 0; j < this.cols; j++)
                        {
                            // Get a pointer to the back buffer.
                            IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                            // Find the address of the pixel to draw.
                            pBackBuffer += i * writeableBitmap.BackBufferStride;
                            pBackBuffer += j * 4;

                            // Compute the pixel's color.
                            int color_data = (byte)(field[i + 1][j + 1] * 255) << 16; // R

                            // Assign the color data to the pixel.
                            *((int*)pBackBuffer) = color_data;
                        }
                    }
                }

                // Specify the area of the bitmap that changed.
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, this.cols, this.rows));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                writeableBitmap.Unlock();
            }

            this.gameEngine.NextGeneration();

            this.stopwatch.Stop();

            this.Title = $"Frame time: {this.stopwatch.ElapsedMilliseconds} ms";
        }
    }
}
