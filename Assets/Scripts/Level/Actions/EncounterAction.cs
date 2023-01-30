using System;
using System.Collections.Generic;
using ExtEvents;
using UnityEngine;

public class EncounterAction : BaseComponent
{
    [Serializable]
    public class EnemySpawnDefinition
    {
        public GameObject prefab;
        public float z;
        public Rotation direction = Rotation.Right;
        public bool partOfWave = true;
    }

    [Serializable]
    public class WaveDefinition
    {
        public EnemySpawnDefinition[] spawnDefinitions;
    }

    public WaveDefinition[] waveDefinitions;
    public GameObject[] firstWavePreSpawnedEnemies;
    public float timeToAlarm;
    public Rect cameraBorder;
    public float spawnSourceDistance = 1;
    [SerializeField] public ExtEvent onWaveStart;
    [SerializeField] public ExtEvent postEncounterEvent;

    private bool stopped;

    public void Invoke()
    {
        stopped = false;
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            var cameraMovement = mainCamera.GetComponent<CameraMovement>();
            cameraMovement.Lock(cameraBorder);
        }

        StartWave(0);
    }

    private void StartWave(int index)
    {
        var waveEnemies = index == 0 ? new List<GameObject>(firstWavePreSpawnedEnemies) : new List<GameObject>();

        foreach (var definition in waveDefinitions[index].spawnDefinitions)
        {
            var enemy = SpawnEnemy(definition);
            if (definition.partOfWave)
            {
                waveEnemies.Add(enemy);
            }
        }

        InvokeWhen(() => waveEnemies.TrueForAll(enemy => !enemy), () =>
        {
            if (!stopped && index + 1 < waveDefinitions.Length)
            {
                StartWave(index + 1);
            }
            else
            {
                FinishEncounter();
            }
        });

        onWaveStart.Invoke();
    }

    private GameObject SpawnEnemy(EnemySpawnDefinition definition)
    {
        var edge = GetEdge(cameraBorder, definition.direction) + definition.direction * spawnSourceDistance;
        var spawnPoint = new Vector3(edge, 0, definition.z);
        var enemy = Instantiate(definition.prefab, GameEntity.ScreenCoordinates(spawnPoint),
            Quaternion.identity);

        var movableObject = enemy.GetComponent<MovableEntity>();
        movableObject.WorldPosition = spawnPoint;

        StartTimeout(() => enemy.GetComponent<AlarmBrainModule>().SetAlarm(), timeToAlarm);

        return enemy;
    }

    private void FinishEncounter()
    {
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            var cameraMovement = mainCamera.GetComponent<CameraMovement>();
            cameraMovement.Unlock();
        }

        postEncounterEvent.Invoke();
    }

    public void Stop()
    {
        stopped = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(cameraBorder.center.x, cameraBorder.center.y, 0.01f),
            new Vector3(cameraBorder.size.x, cameraBorder.size.y, 0.01f));

        Gizmos.color = Color.green;
        for (var wave = 0; wave < waveDefinitions.Length; wave++)
        {
            foreach (var definition in waveDefinitions[wave].spawnDefinitions)
            {
                var position = new Vector3(GetEdge(cameraBorder, definition.direction), 0, definition.z);
                Gizmos.DrawWireSphere(GameEntity.ScreenCoordinates(position), 0.1f * (wave + 1));
            }
        }
    }

    private static float GetEdge(Rect rect, Rotation direction)
    {
        return rect.center.x + direction * rect.size.x / 2;
    }
}