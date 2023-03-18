using System;
using System.Collections;
using UnityEngine;

public class SpikeBallAttack : BaseAttack
{
    [Serializable]
    public class AttackFlow
    {
        public float anticipationDuration;
        public float activeTimeout;
        public float recoveryDuration;
    }
    
    public AttackFlow attackFlow;
    public GameObject ballPrefab;
    public Vector3 ballSpawnPoint;
    public float ballSpeed;

    private SpikeBall ballInstance;
    private float activeStartTime;
    private bool released;
    
    protected override IEnumerator AnticipationPhase()
    {
        yield return new WaitForSeconds(attackFlow.anticipationDuration);
    }

    protected override IEnumerator ActivePhase()
    {
        activeStartTime = Time.time;
        released = false;
        var ballEntity = GameEntity.Instantiate(ballPrefab, Entity.TransformToWorld(ballSpawnPoint));
        ballInstance = ballEntity.GetBehaviour<SpikeBall>();
        ballInstance.Fire(ballSpeed * Entity.WorldRotation * Vector3.right, this);
        yield return new WaitUntil(() => released || Time.time - attackFlow.activeTimeout > activeStartTime);
        ballInstance.Explode();
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(attackFlow.recoveryDuration);
    }

    public void ReleaseBall()
    {
        released = true;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GameEntity.ScreenCoordinates(Entity.TransformToWorld(ballSpawnPoint)), 0.2f);
    }
}