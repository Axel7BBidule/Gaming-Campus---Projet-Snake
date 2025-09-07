using Raylib_CsLo;
using System.Numerics;
using System.Collections.Generic;

public class Snake
{
    public List<Vector2> Body;
    public Vector2 Direction;

    public Snake(Vector2 startPos)
    {
        Body = new List<Vector2>();
        Body.Add(startPos);
        Direction = new Vector2(1, 0);
    }

    public void ChangeDirection(Vector2 newDir)
    {
        if (newDir != Vector2.Zero)
        {
            Direction = newDir;
        }
    }

    public void Move(bool grow = false)
    {
        Vector2 Head = Body[0] + Direction;
        Body.Insert(0, Head);
        if (!grow)
            Body.RemoveAt(Body.Count - 1);
    }

    public bool CollidesWithSelf()
    {
        for (int i = 1; i < Body.Count; i++)
        {
            if (Body[i].Equals(Body[0]))
                return true;
        }
        return false;
    }

    public void Draw(int cellSize)
    {
        for (int i = 0; i < Body.Count; i++)
        {
            Raylib_CsLo.Color c = i == 0 ? Raylib.DARKGREEN : Raylib.GREEN;
            Raylib.DrawRectangle((int)Body[i].X * cellSize, (int)Body[i].Y * cellSize, cellSize, cellSize, c);
        }
    }
}