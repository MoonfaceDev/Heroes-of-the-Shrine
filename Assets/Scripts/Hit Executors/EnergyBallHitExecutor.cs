using System;
using UnityEngine;

[Serializable]
public class EnergyBallExecutor : IHitExecutor
{
    public GameObject ballPrefab;
    public float ballSpawnElevation;
    public float ballExplodeDelay;

    public void Execute(Hit hit)
    {
        var ballEntity = GameEntity.Instantiate(ballPrefab, hit.Victim.RelatedEntity.WorldPosition);
        var ballInstance = ballEntity.GetBehaviour<SpikeBall>();
        ballInstance.Latch(new Collision(hit.Victim, hit.Victim.RelatedEntity.position+ballSpawnElevation*Vector3.up), hit.Direction, hit.Source);
        ballInstance.ExplodeAfter(ballExplodeDelay);
    }
}