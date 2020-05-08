using UnityEngine;
using System.Drawing;
using System;

public static class PointExtension
{
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
}
