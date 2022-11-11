using System;

namespace GameOfLife
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

            this.field = new byte[rows + 2][];
            this.newField = new byte[rows + 2][];

            for (int i = 0; i < rows + 2; i++)
            {
                this.field[i] = new byte[cols + 2];
                this.newField[i] = new byte[cols + 2];
            }

            Random rand = new Random();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    this.field[i + 1][j + 1] = (rand.Next(density) == 0) ? (byte)1 : (byte)0;
                }
            }
        }

        public uint CurrentGeneration { get; private set; }

        public void NextGeneration()
        {
            for (int i = 0; i < this.rows; i++)
            {
                int buffer = 0;

                buffer |= this.field[i][0];
                buffer |= this.field[i + 1][0] << 1;
                buffer |= this.field[i + 2][0] << 2;

                buffer |= this.field[i][1] << 3;
                buffer |= this.field[i + 1][1] << 4;
                buffer |= this.field[i + 2][1] << 5;

                for (int j = 0; j < this.cols; j++)
                {
                    buffer |= this.field[i][j + 2] << 6;
                    buffer |= this.field[i + 1][j + 2] << 7;
                    buffer |= this.field[i + 2][j + 2] << 8;

                    this.newField[i + 1][j + 1] = Magic.Solutions[buffer];

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
                this.field[i + 1][j + 1] = state ? (byte)1 : (byte)0;
            }
        }
    }
}
