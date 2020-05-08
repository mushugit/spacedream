using System.Drawing;
using UnityEngine;

public static class Vector2Extension
{
    public static Point ToPoint(this Vector2 vector2)
    {
        return new Point(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
    }
}

