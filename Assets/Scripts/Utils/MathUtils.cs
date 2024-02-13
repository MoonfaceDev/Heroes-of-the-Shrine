using System;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    private struct LineParams
    {
        public readonly float a;
        public readonly float b;
        public readonly float c;

        public LineParams(float a, float b, float c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }

    private static bool IsBetween(Vector2 start, Vector2 end, Vector2 point)
    {
        return Mathf.Abs(Vector2.Distance(start, point) + Vector2.Distance(end, point) - Vector2.Distance(start, end)) <
               0.001f;
    }

    private static LineParams GetLineParams(Vector2 start, Vector2 end)
    {
        var a = start.y - end.y;
        var b = end.x - start.x;
        var c = start.x * end.y - end.x * start.y;
        return new LineParams(a, b, -c);
    }

    private static bool LineLineIntersection(out Vector2 intersection, Vector2 start1, Vector2 end1, Vector2 start2,
        Vector2 end2)
    {
        var l1 = GetLineParams(start1, end1);
        var l2 = GetLineParams(start2, end2);
        var d = l1.a * l2.b - l1.b * l2.a;
        var dx = l1.c * l2.b - l1.b * l2.c;
        var dy = l1.a * l2.c - l1.c * l2.a;

        if (d == 0)
        {
            intersection = Vector2.zero;
            return false;
        }

        intersection = new Vector2(dx / d, dy / d);
        return IsBetween(start1, end1, intersection) && IsBetween(start2, end2, intersection);
    }

    public static IEnumerable<Vector2> LineRectangleIntersections(Vector2 start, Vector2 end, Vector2 rectanglePosition,
        Vector2 rectangleSize)
    {
        List<Vector2> intersections = new();
        var bottomLeft = rectanglePosition + rectangleSize / 2 * new Vector2(-1, -1);
        var topLeft = rectanglePosition + rectangleSize / 2 * new Vector2(-1, 1);
        var topRight = rectanglePosition + rectangleSize / 2 * new Vector2(1, 1);
        var bottomRight = rectanglePosition + rectangleSize / 2 * new Vector2(1, -1);

        Tuple<Vector2, Vector2>[] lines =
        {
            new(bottomLeft, topLeft),
            new(topLeft, topRight),
            new(topRight, bottomRight),
            new(bottomRight, bottomLeft),
        };
        foreach (var line in lines)
        {
            var hasIntersection = LineLineIntersection(out var intersection, start, end, line.Item1, line.Item2);
            if (hasIntersection)
            {
                intersections.Add(intersection);
            }
        }

        return intersections;
    }

    public static Vector2 ToPlane(Vector3 point)
    {
        return new Vector2(point.x, point.z);
    }

    public static Vector3 ToSpace(Vector2 point, float y = 0)
    {
        return new Vector3(point.x, y, point.y);
    }
}