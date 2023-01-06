using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class Particles : BaseComponent
{
    public ParticleSystem prefab;
    public Vector3 particlePosition;
    public bool attachToCharacter;

    public void Play()
    {
        var movableObject = GetComponent<MovableObject>();
        var clone = attachToCharacter ? Instantiate(prefab, movableObject.transform) : Instantiate(prefab);
        clone.transform.position = MovableObject.ScreenCoordinates(movableObject.TransformToWorld(particlePosition));

        if (!attachToCharacter)
        {
            clone.transform.rotation = movableObject.WorldRotation;
        }

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