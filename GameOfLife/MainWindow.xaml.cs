using Life;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static WriteableBitmap writeableBitmap;
        private GameEngine gameEngine;

        private int rows;
        private int cols;

        private bool completed = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private static void DrawPixel(int row, int column, byte color)
        {
            try
            {
                // Reserve the back buffer for updates.
                writeableBitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                    // Find the address of the pixel to draw.
                    pBackBuffer += row * writeableBitmap.BackBufferStride;
                    pBackBuffer += column * 4;

                    // Compute the pixel's color.
                    int color_data = color << 16; // R

                    // Assign the color data to the pixel.
                    *((int*)pBackBuffer) = color_data;
                }

                // Specify the area of the bitmap that changed.
                writeableBitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                writeableBitmap.Unlock();
            }
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

            using BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += this.Worker_DoWork;
            worker.ProgressChanged += this.Worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (completed)
                {
                    completed = false;

                    ((BackgroundWorker)sender).ReportProgress(0);

                    this.gameEngine.NextGeneration();
                }
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            byte[][] field = this.gameEngine.GetCurrentGeneration();

            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    DrawPixel(i, j, (byte)(field[i + 1][j + 1] * 255));
                }
            }
        }
    }
}
