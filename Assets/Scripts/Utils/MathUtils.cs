using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
	public static bool LineLineIntersection(out Vector2 intersection, Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
	{
		Vector3 vec1 = end1 - start1;
		Vector3 vec2 = end2 - start2;
		Vector3 vec3 = start2 - start1;
		Vector3 crossVec1and2 = Vector3.Cross(vec1, vec2);
		Vector3 crossVec3and2 = Vector3.Cross(vec3, vec2);

		float planarFactor = Vector3.Dot(vec3, crossVec1and2);

		//is coplanar, and not parrallel
		if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
		{
			float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
			intersection = start1 + (vec1 * s);
			return true;
		}
		else
		{
			intersection = Vector3.zero;
			return false;
		}
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
			float sideLength = Vector2.Distance(line.Item1, line.Item2);
			float segmentLength = Vector2.Distance(start, end);
            if (hasIntersection && Vector2.Distance(line.Item1, intersection) < sideLength + offset && Vector2.Distance(line.Item2, intersection) < sideLength + offset && Vector2.Distance(start, intersection) < segmentLength + offset && Vector2.Distance(end, intersection) < segmentLength + offset)
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