namespace GameOfLife
{
    internal struct Cell
    {
        public int PositionX;
        public int PositionY;
        public int Age;

        public bool IsAlive;

        public Cell(int row, int column, int age, bool alive)
        {
            PositionX = row * 5;
            PositionY = column * 5;
            Age = age;
            IsAlive = alive;
        }
    }
}