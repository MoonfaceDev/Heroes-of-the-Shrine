using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class AttackParticles : MonoBehaviour
{
    public ParticleSystem prefab;
    public Vector3 particlePosition;

    public void Play()
    {
        var movableObject = GetComponent<MovableObject>();
        var clone = Instantiate(prefab);
        var shape = clone.GetComponent<ParticleSystem>().shape;
        shape.position = MovableObject.ScreenCoordinates(movableObject.TransformToWorld(particlePosition));
        shape.rotation += movableObject.WorldRotation; 
        clone.GetComponent<Renderer>().sortingOrder = movableObject.SortingOrder + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        var movableObject = GetComponent<MovableObject>();
        Gizmos.DrawWireSphere(MovableObject.ScreenCoordinates(movableObject.TransformToWorld(particlePosition)), 0.1f);
    }
}