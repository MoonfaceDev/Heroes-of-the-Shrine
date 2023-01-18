using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnAttack : BaseAttack
{
    [Serializable]
    public class AttackFlow
    {
        public float anticipationDuration;
        public float recoveryDuration;
    }

    public AttackFlow attackFlow;
    public int maxEnemyCount;
    public GameObject enemyPrefab;
    public Vector3[] spawnPoints;

    protected override IEnumerator AnticipationPhase()
    {
        yield return new WaitForSeconds(attackFlow.anticipationDuration);
    }

    protected override IEnumerator ActivePhase()
    {
        SpawnGoblins();
        yield return new WaitForSeconds(0);
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(attackFlow.recoveryDuration);
    }

    private void SpawnGoblins()
    {
        var currentEnemyCount = EntityManager.Instance.CountEntities(Tag.Enemy);
        var spawnCount = maxEnemyCount - currentEnemyCount;
        var selectedPoints = GetRandomSpawnPoints(spawnCount);
        for (var i = 0; i < maxEnemyCount - currentEnemyCount; i++)
        {
            var newEnemy = Instantiate(enemyPrefab, GameEntity.ScreenCoordinates(selectedPoints[i]),
                Quaternion.identity);
            newEnemy.GetComponent<MovableEntity>().WorldPosition = selectedPoints[i];
            newEnemy.GetComponent<AlarmBrainModule>().SetAlarm();
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

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        foreach (var point in spawnPoints)
        {
            Gizmos.DrawWireSphere(GameEntity.ScreenCoordinates(point), 0.2f);
        }
    }
}