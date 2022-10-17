using Life;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static WriteableBitmap writeableBitmap;

        private readonly DispatcherTimer timer = new DispatcherTimer();

        private readonly Stopwatch stopwatch = new Stopwatch();

        private GameEngine gameEngine;

        private int rows;
        private int cols;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.rows = (int)this.ActualHeight;
            this.cols = (int)this.ActualWidth;

            this.gameEngine = new GameEngine(this.rows, this.cols, 2);


            writeableBitmap = new WriteableBitmap(
                (int)this.ActualWidth,
                (int)this.ActualHeight,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            image.Source = writeableBitmap;

            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            this.timer.Tick += (sender, e) => this.Timer_Tick();
            this.timer.Start();
        }

        private void Timer_Tick()
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
