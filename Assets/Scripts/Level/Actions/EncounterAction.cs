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

public class EncounterAction : BaseAction
{
    public EnemySpawnDefinition[] spawnDefinitions;
    public GameObject[] preSpawnedEnemies;
    public float timeToAlarm;
    public Rect cameraBorder;

    public float spawnSourceDistance = 1;
    public float spawnDestinationDistance = 1;
    public float entranceVelocity = 3;

    public override void Invoke()
    {
        EventManager eventManager = FindObjectOfType<EventManager>();

        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.Lock(cameraBorder);

        List<GameObject> waveEnemies = new(preSpawnedEnemies);

        foreach (EnemySpawnDefinition definition in spawnDefinitions)
        {
            GameObject enemy = Instantiate(definition.prefab);
            MovableObject movableObject = enemy.GetComponent<MovableObject>();

            UncontrolledBehaviour uncontrolledBehaviour = enemy.GetComponent<UncontrolledBehaviour>();
            uncontrolledBehaviour.Play();

            if (definition.direction == Direction.Left)
            {
                movableObject.position.x = cameraBorder.xMin - spawnSourceDistance;
                EventListener entranceEvent = eventManager.Attach(() => true, () => {
                    movableObject.position.x += Time.deltaTime * entranceVelocity;
                }, false);
                eventManager.Attach(() => movableObject.position.x > cameraBorder.xMin + spawnDestinationDistance, () =>
                {
                    eventManager.Detach(entranceEvent);
                    uncontrolledBehaviour.Stop();
                });
            }
            else if (definition.direction == Direction.Right)
            {
                movableObject.position.x = cameraBorder.xMax + spawnSourceDistance;
                EventListener entranceEvent = eventManager.Attach(() => true, () => {
                    movableObject.position.x -= Time.deltaTime * entranceVelocity;
                }, false);
                eventManager.Attach(() => movableObject.position.x < cameraBorder.xMax - spawnDestinationDistance, () =>
                {
                    eventManager.Detach(entranceEvent);
                    uncontrolledBehaviour.Stop();
                });
            }

            movableObject.position.z = definition.z;

            if (definition.partOfWave)
            {
                waveEnemies.Add(enemy);
            }
            else
            {
                enemy.GetComponent<EnemyBrain>().Alarm();
            }
        }

        eventManager.StartTimeout(() =>
        {
            foreach(GameObject enemy in waveEnemies)
            {
                enemy.GetComponent<EnemyBrain>().Alarm();
            }
        }, timeToAlarm);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(cameraBorder.center.x, cameraBorder.center.y, 0.01f), new Vector3(cameraBorder.size.x, cameraBorder.size.y, 0.01f));

        Gizmos.color = Color.green;
        foreach (EnemySpawnDefinition definition in spawnDefinitions)
        {
            if (definition.direction == Direction.Left)
            {
                Gizmos.DrawSphere(MovableObject.ScreenCoordinates(new(cameraBorder.xMin, 0, definition.z)), 0.1f);
            }
            else if (definition.direction == Direction.Right)
            {
                Gizmos.DrawSphere(MovableObject.ScreenCoordinates(new(cameraBorder.xMax, 0, definition.z)), 0.1f);
            }
        }
    }
}
