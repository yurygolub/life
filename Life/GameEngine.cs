using System;

namespace WindowsFormsApp3
{
    internal class GameEngine
    {
        private readonly int rows;
        private readonly int cols;

        private byte[][] field;
        private byte[][] newField;

        public GameEngine(int rows, int cols, int density)
        {
            this.rows = rows;
            this.cols = cols;
            this.field = new byte[rows][];

            this.newField = new byte[rows][];

            Random rand = new Random();
            for (int x = 0; x < rows; x++)
            {
                this.field[x] = new byte[cols];

                this.newField[x] = new byte[cols];

                for (int y = 0; y < cols; y++)
                {
                    this.field[x][y] = (rand.Next(density) == 0) ? (byte)1 : (byte)0;
                }
            }
        }

        public uint CurrentGeneration { get; private set; }

        public void NextGeneration()
        {
            for (int x = 0; x < this.rows; x++)
            {
                for (int y = 0; y < this.cols; y++)
                {
                    int neighboursCount = 0;

                    neighboursCount += this.field[(x - 1 + this.rows) % this.rows][(y - 1 + this.cols) % this.cols];
                    neighboursCount += this.field[(x + this.rows) % this.rows][(y - 1 + this.cols) % this.cols];
                    neighboursCount += this.field[(x + 1 + this.rows) % this.rows][(y - 1 + this.cols) % this.cols];

                    neighboursCount += this.field[(x - 1 + this.rows) % this.rows][(y + this.cols) % this.cols];
                    neighboursCount += this.field[(x + 1 + this.rows) % this.rows][(y + this.cols) % this.cols];

                    neighboursCount += this.field[(x - 1 + this.rows) % this.rows][(y + 1 + this.cols) % this.cols];
                    neighboursCount += this.field[(x + this.rows) % this.rows][(y + 1 + this.cols) % this.cols];
                    neighboursCount += this.field[(x + 1 + this.rows) % this.rows][(y + 1 + this.cols) % this.cols];

                    var hasLife = this.field[x][y] == 1;
                    if (!hasLife && neighboursCount == 3)
                    {
                        this.newField[x][y] = 1;
                    }
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                    {
                        this.newField[x][y] = 0;
                    }
                    else
                    {
                        this.newField[x][y] = this.field[x][y];
                    }
                }
            }

            var temp = this.field;
            this.field = this.newField;
            this.newField = temp;

            this.CurrentGeneration++;
        }

        public byte[][] GetCurrentGeneration()
        {
            return this.field;
        }

        public void AddCell(int x, int y)
        {
            this.UpdateCell(x, y, state: true);
        }

        public void RemoveCell(int x, int y)
        {
            this.UpdateCell(x, y, state: false);
        }

        private bool ValidateCellPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < this.rows && y < this.cols;
        }

        private void UpdateCell(int x, int y, bool state)
        {
            if (this.ValidateCellPosition(x, y))
            {
                this.field[y][x] = state ? (byte)1 : (byte)0;
            }
        }
    }
}
