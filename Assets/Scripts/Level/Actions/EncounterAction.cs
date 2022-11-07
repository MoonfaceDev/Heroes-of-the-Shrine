using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Direction
{
    Left,
    Right,
}

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

public class EncounterAction : MonoBehaviour
{
    public WaveAnnouncer waveAnnouncerPrefab;
    public WaveDefinition[] waveDefinitions;
    public GameObject[] firstWavePreSpawnedEnemies;
    public float timeToAlarm;
    public Rect cameraBorder;
    public float spawnSourceDistance = 1;
    public UnityEvent postEncounterEvent;

    private bool stopped;

    public void Invoke()
    {
        CameraMovement cameraMovement = Camera.main.GetComponent<CameraMovement>();
        cameraMovement.Lock(cameraBorder);

        StartWave(0);
    }

    private void StartWave(int index)
    {
        EventManager eventManager = FindObjectOfType<EventManager>();
        WaveDefinition wave = waveDefinitions[index];

        if (waveAnnouncerPrefab)
        {
            WaveAnnouncer waveAnnouncer = Instantiate(waveAnnouncerPrefab);
            waveAnnouncer.Activate(index);
        }

        List<GameObject> waveEnemies = index == 0 ? new(firstWavePreSpawnedEnemies) : new();

        foreach (EnemySpawnDefinition definition in wave.spawnDefinitions)
        {
            int direction = GetDirection(definition.direction);
            float borderEdge = direction == -1 ? cameraBorder.xMin : cameraBorder.xMax;
            GameObject enemy = Instantiate(definition.prefab);
            MovableObject movableObject = enemy.GetComponent<MovableObject>();

            movableObject.WorldPosition = new(borderEdge + direction * spawnSourceDistance, 0, definition.z);

            if (definition.partOfWave)
            {
                waveEnemies.Add(enemy);
            }
            else
            {
                eventManager.StartTimeout(() => enemy.GetComponent<EnemyBrain>().Alarm(), timeToAlarm);
            }
        }

        eventManager.StartTimeout(() =>
        {
            foreach (GameObject enemy in waveEnemies)
            {
                if (enemy)
                {
                    enemy.GetComponent<EnemyBrain>().Alarm();
                }
            }
        }, timeToAlarm);

        eventManager.Attach(() => waveEnemies.TrueForAll(enemy => !enemy), () =>
        {
            if (!stopped && index + 1 < waveDefinitions.Length)
            {
                StartWave(index + 1);
            }
            else
            {
                CameraMovement cameraMovement = Camera.main.GetComponent<CameraMovement>();
                cameraMovement.Unlock();
                postEncounterEvent.Invoke();
            }
        });
    }

    public void Stop()
    {
        stopped = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(cameraBorder.center.x, cameraBorder.center.y, 0.01f), new Vector3(cameraBorder.size.x, cameraBorder.size.y, 0.01f));

        Gizmos.color = Color.green;
        for (int wave = 0; wave < waveDefinitions.Length; wave++)
        {
            foreach (EnemySpawnDefinition definition in waveDefinitions[wave].spawnDefinitions)
            {
                if (definition.direction == Direction.Left)
                {
                    Gizmos.DrawWireSphere(MovableObject.ScreenCoordinates(new(cameraBorder.xMin, 0, definition.z)), 0.1f * (wave + 1));
                }
                else if (definition.direction == Direction.Right)
                {
                    Gizmos.DrawWireSphere(MovableObject.ScreenCoordinates(new(cameraBorder.xMax, 0, definition.z)), 0.1f * (wave + 1));
                }
            }
        }
    }

    private static int GetDirection(Direction direction)
    {
        if (direction == Direction.Left)
        {
            return -1;
        }
        if (direction == Direction.Right)
        {
            return 1;
        }
        return 0;
    }
}
