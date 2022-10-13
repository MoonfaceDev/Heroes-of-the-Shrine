using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
	public static bool IsBetween(Vector2 start, Vector2 end, Vector2 point)
    {
		return Mathf.Sign(point.x - start.x) != Mathf.Sign(point.x - end.x) && Mathf.Sign(point.y - start.y) != Mathf.Sign(point.y - end.y);
	}

	public static bool LineLineIntersection(out Vector2 intersection, Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
	{
		float m1 = (end1.y - start1.y) / (end1.x - start1.x);
		float m2 = (end2.y - start2.y) / (end2.x - start2.x);
		float b1 = start1.y - m1 * start1.x;
		float b2 = start2.y - m2 * start2.x;

		// y = m1 * x + b1
		// y = m2 * x + b2
		if (Mathf.Abs(m1 - m2) < Mathf.Epsilon) // parallel
        {
			if (Mathf.Abs(b1 - b2) < Mathf.Epsilon) // identical
            {
				intersection = start1;
				return true;
            }
			else
            {
				intersection = Vector2.zero;
				return false;
			}
        }

		float x = (b2 - b1) / (m1 - m2);
		float y = m1 * x + b1;

		intersection = new Vector2(x, y);
		return IsBetween(start1, end1, intersection) && IsBetween(start2, end2, intersection);
	}

	public static List<Vector2> LineRectangleIntersections(Vector2 start, Vector2 end, Vector2 rectanglePosition, Vector2 rectangleSize, float offset = 0)
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