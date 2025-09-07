using Raylib_CsLo;
using System.Numerics;


public enum AppleColor
{
    Rouge, Bleu, Vert, Noir, Jaune, Violet, Orange, Rose,
    Cyan, Marron, Gris, Blanc
}

public record Apple(Vector2 Position, AppleColor Color)
{
    public void Draw(int cellSize, Raylib_CsLo.Color color)
    {
        Raylib.DrawRectangle((int)Position.X * cellSize, (int)Position.Y * cellSize, cellSize, cellSize, color);
    }
}
