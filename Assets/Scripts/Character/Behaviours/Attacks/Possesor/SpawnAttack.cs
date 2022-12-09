using System.Collections;
using System.Linq;
using UnityEngine;

public class SpawnAttack : BaseAttack
{
    public float anticipateDuration;
    public float recoveryDuration;
    public int maxEnemyCount;
    public GameObject enemyPrefab;
    public Vector3[] spawnPoints;

    public override bool CanPlay()
    {
        return base.CanPlay() &&
               !((AttackManager.Anticipating || AttackManager.Active || AttackManager.HardRecovering) &&
                 !(instant && AttackManager.IsInterruptible()));
    }

    public override void Awake()
    {
        base.Awake();
        OnPlay += () =>
        {
            DisableBehaviours(typeof(WalkBehaviour));
            StopBehaviours(typeof(WalkBehaviour));
            MovableObject.velocity = Vector3.zero;
        };
        generalEvents.onFinishRecovery.AddListener(() => EnableBehaviours(typeof(WalkBehaviour)));
        OnStop += () => EnableBehaviours(typeof(WalkBehaviour));

        generalEvents.onStartActive.AddListener(SpawnGoblins);
    }

    private void SpawnGoblins()
    {
        var currentEnemyCount = CachedObjectsManager.Instance.GetObjects<Character>("Enemy").Length;
        var spawnCount = maxEnemyCount - currentEnemyCount;
        var selectedPoints = GetRandomSpawnPoints(spawnCount);
        for (var i = 0; i < maxEnemyCount - currentEnemyCount; i++)
        {
            var newEnemy = Instantiate(enemyPrefab, MovableObject.ScreenCoordinates(selectedPoints[i]), Quaternion.identity);
            newEnemy.GetComponent<MovableObject>().WorldPosition = selectedPoints[i];
            newEnemy.GetComponent<EnemyBrain>().Alarm();
        }
    }

    private Vector3[] GetRandomSpawnPoints(int count)
    {
        var availablePoints = spawnPoints.ToList();
        var selectedPoints = new Vector3[count];

        for (var i = 0; i < count; i++)
        {
            var randomIndex = Random.Range(0, availablePoints.Count);
            selectedPoints[i] = availablePoints[randomIndex];
            availablePoints.RemoveAt(randomIndex);
        }

        return selectedPoints;
    }

    protected override IEnumerator AnticipateCoroutine()
    {
        yield return new WaitForSeconds(anticipateDuration);
    }

    protected override IEnumerator ActiveCoroutine()
    {
        yield return new WaitForSeconds(0);
    }

    protected override IEnumerator RecoveryCoroutine()
    {
        yield return new WaitForSeconds(recoveryDuration);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        foreach (var point in spawnPoints)
        {
            Gizmos.DrawWireSphere(MovableObject.ScreenCoordinates(point), 0.2f);
        }
    }
}