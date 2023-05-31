namespace Board
{
    public class Board
    {
        public int Width;
        public int Height;
        public int[] Grid;

        public Board(){}
        
        public Board(int width, int height, int[] grid)
        {
            Width = width;
            Height = height;
            Grid = grid;
        }
    }
}
