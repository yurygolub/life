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
                int buffer = 0;

                buffer |= this.field[(i - 1 + this.rows) % this.rows][(-1 + this.cols) % this.cols];
                buffer |= this.field[(i + this.rows) % this.rows][(-1 + this.cols) % this.cols] << 1;
                buffer |= this.field[(i + 1 + this.rows) % this.rows][(-1 + this.cols) % this.cols] << 2;

                buffer |= this.field[(i - 1 + this.rows) % this.rows][0] << 3;
                buffer |= this.field[(i + this.rows) % this.rows][0] << 4;
                buffer |= this.field[(i + 1 + this.rows) % this.rows][0] << 5;

                for (int j = 0; j < this.cols; j++)
                {
                    buffer |= this.field[(i - 1 + this.rows) % this.rows][(j + 1 + this.cols) % this.cols] << 6;
                    buffer |= this.field[(i + this.rows) % this.rows][(j + 1 + this.cols) % this.cols] << 7;
                    buffer |= this.field[(i + 1 + this.rows) % this.rows][(j + 1 + this.cols) % this.cols] << 8;

                    this.newField[i][j] = Magic.Solutions[buffer];

                    buffer >>= 3;
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
