using System;
using System.Collections.Generic;
using UnityEngine;

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

public class EncounterAction : BaseAction
{
    public WaveDefinition[] waveDefinitions;
    public GameObject[] firstWavePreSpawnedEnemies;
    public float timeToAlarm;
    public Rect cameraBorder;

    public float spawnSourceDistance = 1;
    public float spawnDestinationDistance = 1;
    public float entranceVelocity = 3;

    public override void Invoke()
    {
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.Lock(cameraBorder);

        StartWave(0);
    }

    private void StartWave(int index)
    {
        EventManager eventManager = FindObjectOfType<EventManager>();

        List<GameObject> waveEnemies = index == 0 ? new(firstWavePreSpawnedEnemies) : new();

        foreach (EnemySpawnDefinition definition in waveDefinitions[index].spawnDefinitions)
        {
            int direction = GetDirection(definition.direction);
            float borderEdge = direction == -1 ? cameraBorder.xMin : cameraBorder.xMax;
            GameObject enemy = Instantiate(definition.prefab);
            MovableObject movableObject = enemy.GetComponent<MovableObject>();

            UncontrolledBehaviour uncontrolledBehaviour = enemy.GetComponent<UncontrolledBehaviour>();
            uncontrolledBehaviour.Play();

            movableObject.position.x = borderEdge + direction * spawnSourceDistance;
            uncontrolledBehaviour.LookDirection = -direction;

            EventListener entranceEvent = eventManager.Attach(() => true, () => {
                movableObject.position.x -= direction * entranceVelocity * Time.deltaTime;
            }, false);

            eventManager.Attach(() => Mathf.Sign(movableObject.position.x - (borderEdge - direction * spawnDestinationDistance)) == -direction, () =>
            {
                eventManager.Detach(entranceEvent);
                uncontrolledBehaviour.Stop();
            });

            movableObject.position.z = definition.z;

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
                enemy.GetComponent<EnemyBrain>().Alarm();
            }
        }, timeToAlarm);

        eventManager.Attach(() => waveEnemies.TrueForAll(enemy => !enemy), () =>
        {
            if (index + 1 < waveDefinitions.Length)
            {
                StartWave(index + 1);
            }
            else
            {
                CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
                cameraFollow.Unlock();
            }
        });
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
