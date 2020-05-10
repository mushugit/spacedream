using System;
using System.Drawing;
using UnityEngine;

public static class PointExtension
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public static int ManhattanDistance(this Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
    public static float EuclidianDistance(this Point a, Point b)
    {
        return Mathf.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));
    }

    public static Vector2 ToVector2(this Point point)
    {
        return new Vector2(point.X, point.Y);
    }

    public static Point GetPointToDirection(this Point p, Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return new Point(p.X, p.Y + 1);
            case Direction.East:
                return new Point(p.X + 1, p.Y);
            case Direction.South:
                return new Point(p.X, p.Y - 1);
            case Direction.West:
                return new Point(p.X - 1, p.Y);
            default:
                return Point.Empty;
        };
    }
}
