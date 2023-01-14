using UnityEngine;

[RequireComponent(typeof(GameEntity))]
public class Particles : BaseComponent
{
    public ParticleSystem prefab;
    public Vector3 particlePosition;
    public bool attachToCharacter;

    public void Play()
    {
        var entity = GetComponent<GameEntity>();
        var clone = attachToCharacter ? Instantiate(prefab, entity.transform) : Instantiate(prefab);
        clone.transform.position = GameEntity.ScreenCoordinates(entity.TransformToWorld(particlePosition));

        if (!attachToCharacter)
        {
            clone.transform.rotation = entity.WorldRotation;
        }

        clone.GetComponent<Renderer>().sortingOrder = entity.SortingOrder + 1;
        clone.GetComponent<ParticleSystem>().Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        var entity = GetComponent<GameEntity>();
        Gizmos.DrawWireSphere(GameEntity.ScreenCoordinates(entity.TransformToWorld(particlePosition)), 0.1f);
    }
}