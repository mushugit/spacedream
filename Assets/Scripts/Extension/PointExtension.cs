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
}
