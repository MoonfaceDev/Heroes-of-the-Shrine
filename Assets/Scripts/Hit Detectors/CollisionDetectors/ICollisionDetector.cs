using System.Collections.Generic;
using UnityEngine;

public record Collision(IHittable Other, Vector3 Point);

public interface ICollisionDetector
{
    public IEnumerable<Collision> Detect();
}