using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#pragma warning disable SA1407

namespace Life
{
    public partial class Form1 : Form
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        private int rows;
        private int cols;

        private int height;
        private int width;

        private int rgbValuesLength;
        private byte[] rgbValues;

        private Graphics graphics;
        private int resolution;
        private GameEngine gameEngine;

        public Form1()
        {
            this.InitializeComponent();
        }

        private void StartGame()
        {
            if (this.timer1.Enabled)
            {
                return;
            }

            this.nudResolution.Enabled = false;
            this.nudDensity.Enabled = false;
            this.resolution = (int)this.nudResolution.Value;

            this.rows = this.pictureBox1.Height / this.resolution;
            this.cols = this.pictureBox1.Width / this.resolution;

            this.gameEngine = new GameEngine(this.rows, this.cols, (int)this.nudDensity.Minimum + (int)this.nudDensity.Maximum - (int)this.nudDensity.Value);

            this.Text = $"Generation {this.gameEngine.CurrentGeneration}";

            this.pictureBox1.Image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.graphics = Graphics.FromImage(this.pictureBox1.Image);

            this.height = this.pictureBox1.Height;
            this.width = this.pictureBox1.Width;

            this.rgbValuesLength = this.height * this.width * 4;
            this.rgbValues = new byte[this.rgbValuesLength];
            for (int i = 0; i < this.rgbValuesLength; i += 4)
            {
                this.rgbValues[i + 3] = 255;
            }

            this.timer1.Start();
        }

        private void DrawNextGeneration()
        {
            this.stopwatch.Restart();

            Bitmap bmp = (Bitmap)this.pictureBox1.Image.Clone();

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

            var field = this.gameEngine.GetCurrentGeneration();
            for (int y = 0; y < this.rows; y++)
            {
                for (int i = 0; i < this.resolution; i++)
                {
                    int yIndex = (y * this.resolution + i) * this.width;
                    for (int x = 0; x < this.cols; x++)
                    {
                        for (int j = 0; j < this.resolution; j++)
                        {
                            this.rgbValues[(yIndex + x * this.resolution + j) * 4 + 2] = (byte)(field[y + 1][x + 1] * 255);
                        }
                    }
                }
            }

            IntPtr ptr = bmpData.Scan0;

            Marshal.Copy(this.rgbValues, 0, ptr, this.rgbValuesLength);

            bmp.UnlockBits(bmpData);

            this.graphics.DrawImage(bmp, 0, 0);

            this.pictureBox1.Refresh();

            this.gameEngine.NextGeneration();

            this.stopwatch.Stop();

            this.Text = $"Generation {this.gameEngine.CurrentGeneration}   Frame time: {this.stopwatch.ElapsedMilliseconds}";
        }

        private void StopGame()
        {
            if (!this.timer1.Enabled)
            {
                return;
            }

            this.timer1.Stop();
            this.nudDensity.Enabled = true;
            this.nudResolution.Enabled = true;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.DrawNextGeneration();
        }

        private void BStart_Click(object sender, EventArgs e)
        {
            this.StartGame();
        }

        private void BStop_Click(object sender, EventArgs e)
        {
            this.StopGame();
        }

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
    }
}
