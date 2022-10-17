﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
	struct LineParams
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

	public static bool IsBetween(Vector2 start, Vector2 end, Vector2 point)
    {
		return Mathf.Abs(Vector2.Distance(start, point) + Vector2.Distance(end, point) - Vector2.Distance(start, end)) < 0.001f;
	}

	private static LineParams GetLineParams(Vector2 start, Vector2 end)
    {
		float a = start.y - end.y;
		float b = end.x - start.x;
		float c = start.x * end.y - end.x * start.y;
		return new(a, b ,-c);
    }

	public static bool LineLineIntersection(out Vector2 intersection, Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
	{
		LineParams l1 = GetLineParams(start1, end1);
		LineParams l2 = GetLineParams(start2, end2);
		float d = l1.a * l2.b - l1.b * l2.a;
		float dx = l1.c * l2.b - l1.b * l2.c;
		float dy = l1.a * l2.c - l1.c * l2.a;

		if (d == 0)
        {
			intersection = Vector2.zero;
			return false;
        }

		intersection = new Vector2(dx / d, dy / d);
		return IsBetween(start1, end1, intersection) && IsBetween(start2, end2, intersection);
	}

    public static List<Vector2> LineRectangleIntersections(Vector2 start, Vector2 end, Vector2 rectanglePosition, Vector2 rectangleSize)
    {
		List<Vector2> intersections = new();
		Tuple<Vector2, Vector2>[] lines = new Tuple<Vector2, Vector2>[] { 
			new (rectanglePosition, rectanglePosition + rectangleSize.y * Vector2.up),
			new (rectanglePosition + rectangleSize.y * Vector2.up, rectanglePosition + rectangleSize),
			new (rectanglePosition + rectangleSize, rectanglePosition + rectangleSize.x * Vector2.right),
			new (rectanglePosition + rectangleSize.x * Vector2.right, rectanglePosition)
		};
		foreach (Tuple<Vector2, Vector2> line in lines)
		{
			bool hasIntersection = LineLineIntersection(out Vector2 intersection, start, end, line.Item1, line.Item2);
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