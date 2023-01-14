using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class HitParticles : BaseComponent
{
    public ParticleSystem prefab;

    public void Play(Vector3 hitPoint, IHittable hittable)
    {
        var movableObject = GetComponent<MovableObject>();
        var clone = Instantiate(prefab);
        clone.transform.position = MovableObject.ScreenCoordinates(hitPoint);
        clone.transform.rotation = movableObject.WorldRotation;
        clone.GetComponent<Renderer>().sortingOrder = Mathf.Max(
            movableObject.parent.SortingOrder,
            hittable.Character.movableObject.SortingOrder
        ) + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }
}