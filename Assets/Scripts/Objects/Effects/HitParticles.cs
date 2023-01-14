using UnityEngine;

[RequireComponent(typeof(GameEntity))]
public class HitParticles : BaseComponent
{
    public ParticleSystem prefab;

    public void Play(Vector3 hitPoint, IHittable hittable)
    {
        var entity = GetComponent<GameEntity>();
        var clone = Instantiate(prefab);
        clone.transform.position = GameEntity.ScreenCoordinates(hitPoint);
        clone.transform.rotation = entity.WorldRotation;
        clone.GetComponent<Renderer>().sortingOrder = Mathf.Max(
            entity.parent.SortingOrder,
            hittable.Character.movableObject.SortingOrder
        ) + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }
}