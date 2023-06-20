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
        var ballEntity = GameEntity.Instantiate(ballPrefab, hit.victim.Character.Entity.WorldPosition);
        var ballInstance = ballEntity.GetBehaviour<SpikeBall>();
        ballInstance.Latch(hit.victim, hit.direction, ballSpawnElevation, hit.source);
        ballInstance.ExplodeAfter(ballExplodeDelay);
    }
}