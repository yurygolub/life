using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using LifeGameService;

#pragma warning disable SA1407

namespace Life
{
    public partial class Form1 : Form
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        private int rows;
        private int cols;

        private int resolution;
        private GameEngine gameEngine;

        public Form1()
        {
            this.InitializeComponent();
        }

        private void StartGame()
        {
            this.ToggleControls();

            this.resolution = (int)this.nudResolution.Value;

            this.rows = this.pictureBox1.Height / this.resolution;
            this.cols = this.pictureBox1.Width / this.resolution;
            int density = (int)this.nudDensity.Minimum + (int)this.nudDensity.Maximum - (int)this.nudDensity.Value;
            this.gameEngine = new GameEngine(this.rows, this.cols, density);

            this.pictureBox1.Image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height, PixelFormat.Format24bppRgb);

            this.timer1.Start();
        }

        private void DrawNextGeneration()
        {
            Bitmap bmp = (Bitmap)this.pictureBox1.Image;

            BitmapData bmpData = null;

            var field = this.gameEngine.GetCurrentField();
            try
            {
                bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                for (int y = 0; y < this.rows; y++)
                {
                    for (int i = 0; i < this.resolution; i++)
                    {
                        int yIndex = y * this.resolution + i;
                        for (int x = 0; x < this.cols; x++)
                        {
                            for (int j = 0; j < this.resolution; j++)
                            {
                                int xIndex = x * this.resolution + j;

                                IntPtr backBufferPtr = bmpData.Scan0;

                                backBufferPtr += yIndex * bmpData.Stride;
                                backBufferPtr += xIndex * 3;

                                int colorData = (byte)(field[y + 1][x + 1] * 255) << 16; // R

                                unsafe
                                {
                                    *(int*)backBufferPtr = colorData;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bmpData);
                this.pictureBox1.Refresh();
            }

            this.gameEngine.NextGeneration();

            this.stopwatch.Stop();

            this.Text = $"Generation {this.gameEngine.CurrentGeneration}   Frame time: {this.stopwatch.ElapsedMilliseconds}";

            this.stopwatch.Restart();
        }

        private void StopGame()
        {
            this.timer1.Stop();
            this.ToggleControls();
        }

        private void Timer1_Tick(object sender, EventArgs e) => this.DrawNextGeneration();

        private void BStart_Click(object sender, EventArgs e) => this.StartGame();

        private void BStop_Click(object sender, EventArgs e) => this.StopGame();

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.timer1.Enabled)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / this.resolution;
                var y = e.Location.Y / this.resolution;
                this.gameEngine.AddCell(x, y);
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / this.resolution;
                var y = e.Location.Y / this.resolution;
                this.gameEngine.RemoveCell(x, y);
            }
        }

        private void ToggleControls()
        {
            this.bStart.Enabled = !this.bStart.Enabled;
            this.bStop.Enabled = !this.bStop.Enabled;
            this.nudResolution.Enabled = !this.nudResolution.Enabled;
            this.nudDensity.Enabled = !this.nudDensity.Enabled;
        }
    }
}
