using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class HitParticles : MonoBehaviour
{
    public ParticleSystem prefab;
    public Vector3 particlePosition;

    public void Play(IHittable hittable)
    {
        var movableObject = GetComponent<MovableObject>();
        var clone = Instantiate(prefab);
        clone.transform.position = MovableObject.ScreenCoordinates(movableObject.TransformToWorld(particlePosition));
        clone.transform.rotation = movableObject.WorldRotation;
        clone.GetComponent<Renderer>().sortingOrder = Mathf.Max(
            movableObject.parent.SortingOrder,
            hittable.Character.movableObject.SortingOrder
        ) + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        var movableObject = GetComponent<MovableObject>();
        Gizmos.DrawWireSphere(MovableObject.ScreenCoordinates(movableObject.TransformToWorld(particlePosition)), 0.1f);
    }
}