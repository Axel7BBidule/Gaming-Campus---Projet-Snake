using Raylib_CsLo;

public class Grid
{
    public int Size { get; }
    public int CellSize { get; }

    public Grid(int size, int cellSize)
    {
        Size = size;
        CellSize = cellSize;
    }

    public void Draw()
    {
        Raylib.DrawRectangle(0, 0, Size * CellSize, Size * CellSize, Raylib.GRAY);
        for (int x = 0; x <= Size; x++)
        {
            Raylib.DrawLine(x * CellSize, 0, x * CellSize, Size * CellSize, Raylib.BLACK);
        }
        for (int y = 0; y <= Size; y++)
        {
            Raylib.DrawLine(0, y * CellSize, Size * CellSize, y * CellSize, Raylib.BLACK);
        }
    }
}