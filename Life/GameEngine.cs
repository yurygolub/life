using System;

namespace Life
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
            for (int i = 0; i < rows; i++)
            {
                this.field[i] = new byte[cols];
                this.newField[i] = new byte[cols];

                for (int j = 0; j < cols; j++)
                {
                    this.field[i][j] = (rand.Next(density) == 0) ? (byte)1 : (byte)0;
                }
            }
        }

        public uint CurrentGeneration { get; private set; }

        public void NextGeneration()
        {
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    int neighboursCount = 0;

                    neighboursCount += this.field[(i - 1 + this.rows) % this.rows][(j - 1 + this.cols) % this.cols];
                    neighboursCount += this.field[(i + this.rows) % this.rows][(j - 1 + this.cols) % this.cols];
                    neighboursCount += this.field[(i + 1 + this.rows) % this.rows][(j - 1 + this.cols) % this.cols];

                    neighboursCount += this.field[(i - 1 + this.rows) % this.rows][(j + this.cols) % this.cols];
                    neighboursCount += this.field[(i + 1 + this.rows) % this.rows][(j + this.cols) % this.cols];

                    neighboursCount += this.field[(i - 1 + this.rows) % this.rows][(j + 1 + this.cols) % this.cols];
                    neighboursCount += this.field[(i + this.rows) % this.rows][(j + 1 + this.cols) % this.cols];
                    neighboursCount += this.field[(i + 1 + this.rows) % this.rows][(j + 1 + this.cols) % this.cols];

                    var hasLife = this.field[i][j] == 1;
                    if (!hasLife && neighboursCount == 3)
                    {
                        this.newField[i][j] = 1;
                    }
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                    {
                        this.newField[i][j] = 0;
                    }
                    else
                    {
                        this.newField[i][j] = this.field[i][j];
                    }
                }
            }

            byte[][] temp = this.field;
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
            this.UpdateCell(y, x, state: true);
        }

        public void RemoveCell(int x, int y)
        {
            this.UpdateCell(y, x, state: false);
        }

        private bool ValidateCellPosition(int i, int j)
        {
            return i >= 0 && j >= 0 && i < this.rows && j < this.cols;
        }

        private void UpdateCell(int i, int j, bool state)
        {
            if (this.ValidateCellPosition(i, j))
            {
                this.field[i][j] = state ? (byte)1 : (byte)0;
            }
        }
    }
}
