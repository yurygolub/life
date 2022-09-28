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
            InitializeComponent();
        }

        private void StartGame()
        {
            if (timer1.Enabled)
            {
                return;
            }            

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            resolution = (int)nudResolution.Value;
            gameEngine = new GameEngine
            (
                rows: pictureBox1.Height / resolution,
                cols: pictureBox1.Width / resolution,
                density: (int)nudDensity.Minimum + (int)nudDensity.Maximum - (int)nudDensity.Value
            );

            Text = $"Generation {gameEngine.CurrentGeneration}";
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        private void DrawNextGeneration()
        {
            stopwatch.Restart();

            graphics.Clear(Color.Black);
            var field = gameEngine.GetCurrentGeneration();
            for (int x = 0; x < field.Length; x++)
            {
                for (int y = 0; y < field[x].Length; y++)
                {
                    if (field[x][y] == 1)
                    {
                        graphics.FillRectangle(Brushes.Crimson, y * resolution, x * resolution, resolution, resolution);
                        //graphics.GetHdc();
                    }
                }
            }

            pictureBox1.Refresh();

            gameEngine.NextGeneration();

            stopwatch.Stop();

            Text = $"Generation {gameEngine.CurrentGeneration}   Frame time: {stopwatch.ElapsedMilliseconds}";
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
            {
                return;
            }

            timer1.Stop();
            nudDensity.Enabled = true;
            nudResolution.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawNextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                gameEngine.AddCell(x, y);
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                gameEngine.RemoveCell(x, y);
            }
        }
    }
}
