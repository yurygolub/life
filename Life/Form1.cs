using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private int resolution;
        private GameEngine gameEngine;

        private Stopwatch stopwatch = new Stopwatch();

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
            this.gameEngine = new GameEngine
            (
                rows: this.pictureBox1.Height / this.resolution,
                cols: this.pictureBox1.Width / this.resolution,
                density: (int)this.nudDensity.Minimum + (int)this.nudDensity.Maximum - (int)this.nudDensity.Value
            );

            this.Text = $"Generation {this.gameEngine.CurrentGeneration}";
            this.pictureBox1.Image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.graphics = Graphics.FromImage(this.pictureBox1.Image);
            this.timer1.Start();
        }

        private void DrawNextGeneration()
        {
            this.stopwatch.Restart();

            this.graphics.Clear(Color.Black);
            var field = this.gameEngine.GetCurrentGeneration();
            for (int x = 0; x < field.Length; x++)
            {
                for (int y = 0; y < field[x].Length; y++)
                {
                    if (field[x][y] == 1)
                    {
                        this.graphics.FillRectangle(Brushes.Crimson, y * this.resolution, x * this.resolution, this.resolution, this.resolution);

                        // graphics.GetHdc();
                    }
                }
            }

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
