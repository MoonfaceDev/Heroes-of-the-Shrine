﻿using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnAttack : BaseAttack
{
    public int maxEnemyCount;
    public GameObject enemyPrefab;
    public Vector3[] spawnPoints;

    protected override IEnumerator ActivePhase()
    {
        SpawnGoblins();
        yield return new WaitForSeconds(0);
    }

    private void SpawnGoblins()
    {
        var currentEnemyCount = EntityManager.Instance.CountEntities(Tag.Enemy);
        var spawnCount = maxEnemyCount - currentEnemyCount;
        var selectedPoints = GetRandomSpawnPoints(spawnCount);
        for (var i = 0; i < maxEnemyCount - currentEnemyCount; i++)
        {
            var newEnemy = GameEntity.Instantiate(enemyPrefab, selectedPoints[i]);
            newEnemy.GetBehaviour<AlarmBrainModule>().SetAlarm();
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