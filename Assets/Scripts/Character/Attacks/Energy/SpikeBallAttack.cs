using System;
using System.Collections;
using UnityEngine;

public class SpikeBallAttack : BaseChargeAttack
{
    [Serializable]
    public class AttackFlow
    {
        public float anticipationDuration;
        public float activeTimeout;
        public float minActiveTime;
        public float recoveryDuration;
    }

    public AttackFlow attackFlow;
    public GameObject ballPrefab;
    public Vector3 ballSpawnPoint;
    public float ballSpeed;

    private SpikeBall ballInstance;
    private float activeStartTime;

    protected override void Awake()
    {
        base.Awake();
        phaseEvents.onFinishActive += () => ballInstance.Explode();
    }

    protected override IEnumerator ActivePhase()
    {
        activeStartTime = Time.time;
        var ballEntity = GameEntity.Instantiate(ballPrefab, Entity.TransformToWorld(ballSpawnPoint));
        ballInstance = ballEntity.GetBehaviour<SpikeBall>();
        ballInstance.Fire(ballSpeed * Entity.WorldRotation * Vector3.right, this);
        yield return base.ActivePhase();
    }

    protected override bool CanRelease()
    {
        return Time.time - activeStartTime > attackFlow.minActiveTime;
    }

    protected override bool MustRelease()
    {
        return Time.time - activeStartTime > attackFlow.activeTimeout;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GameEntity.ScreenCoordinates(Entity.TransformToWorld(ballSpawnPoint)), 0.2f);
    }
}