using System;

namespace WindowsFormsApp3
{
    class GameEngine
    {
        private readonly int rows;
        private readonly int cols;

        private byte[][] field;
        private byte[][] newField;

        public GameEngine(int rows, int cols, int density)
        {
            this.rows = rows;
            this.cols = cols;
            field = new byte[rows][];

            newField = new byte[rows][];

            Random rand = new Random();
            for (int x = 0; x < rows; x++)
            {
                field[x] = new byte[cols];

                newField[x] = new byte[cols];

                for (int y = 0; y < cols; y++)
                {
                    field[x][y] = (rand.Next(density) == 0) ? (byte)1 : (byte)0;
                }
            }
        }

        public uint CurrentGeneration { get; private set; }

        public void NextGeneration()
        {
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    int neighboursCount = 0;

                    neighboursCount += field[(x - 1 + rows) % rows][(y - 1 + cols) % cols];
                    neighboursCount += field[(x + rows) % rows][(y - 1 + cols) % cols];
                    neighboursCount += field[(x + 1 + rows) % rows][(y - 1 + cols) % cols];

                    neighboursCount += field[(x - 1 + rows) % rows][(y + cols) % cols];
                    neighboursCount += field[(x + 1 + rows) % rows][(y + cols) % cols];

                    neighboursCount += field[(x - 1 + rows) % rows][(y + 1 + cols) % cols];
                    neighboursCount += field[(x + rows) % rows][(y + 1 + cols) % cols];
                    neighboursCount += field[(x + 1 + rows) % rows][(y + 1 + cols) % cols];

                    var hasLife = field[x][y] == 1;
                    if (!hasLife && neighboursCount == 3)
                    {
                        newField[x][y] = 1;
                    }
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                    {
                        newField[x][y] = 0;
                    }
                    else
                    {
                        newField[x][y] = field[x][y];
                    }                    
                }
            }

            var temp = field;
            field = newField;
            newField = temp;

            CurrentGeneration++;
        }

        public byte[][] GetCurrentGeneration()
        {
            return field;
        }

        private bool ValidateCellPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < rows && y < cols;
        }

        private void UpdateCell(int x, int y, bool state)
        {
            if(ValidateCellPosition(x, y))
            {
                field[y][x] = state ? (byte)1 : (byte)0;
            }
        }

        public void AddCell(int x, int y)
        {
            UpdateCell(x, y, state: true);
        }

        public void RemoveCell(int x, int y)
        {
            UpdateCell(x, y, state: false);
        }
    }
}
