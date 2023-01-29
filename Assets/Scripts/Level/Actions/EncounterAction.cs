using System;
using System.Collections.Generic;
using ExtEvents;
using UnityEngine;

public enum Direction
{
    Left,
    Right
}

public class EncounterAction : BaseComponent
{
    [Serializable]
    public class EnemySpawnDefinition
    {
        public GameObject prefab;
        public Direction direction;
        public float z;
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
        var direction = GetDirection(definition.direction);
        var borderEdge = direction == -1 ? cameraBorder.xMin : cameraBorder.xMax;
        var spawnPoint = new Vector3(borderEdge + direction * spawnSourceDistance, 0, definition.z);
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
                switch (definition.direction)
                {
                    case Direction.Left:
                        Gizmos.DrawWireSphere(
                            GameEntity.ScreenCoordinates(new Vector3(cameraBorder.xMin, 0, definition.z)),
                            0.1f * (wave + 1));
                        break;
                    case Direction.Right:
                        Gizmos.DrawWireSphere(
                            GameEntity.ScreenCoordinates(new Vector3(cameraBorder.xMax, 0, definition.z)),
                            0.1f * (wave + 1));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private static int GetDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };
    }
}