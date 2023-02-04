using System;
using System.Collections.Generic;
using System.Linq;
using ExtEvents;
using UnityEngine;

/// <summary>
/// Class that manages a single encounter in the scene. An encounter can have multiple waves.
/// </summary>
public class EncounterAction : BaseComponent
{
    /// <summary>
    /// Definition of a single enemy spawning in a wave
    /// </summary>
    [Serializable]
    public class EnemySpawnDefinition
    {
        /// <value>
        /// Prefab of the enemy
        /// </value>
        public GameObject prefab;

        /// <value>
        /// Side of the camera to spawn in
        /// </value>
        public Rotation direction = Rotation.Normal;

        /// <value>
        /// Z axis value in which enemy spawns
        /// </value>
        public float z;

        /// <value>
        /// If <c>true</c>, this enemy has to be killed for the wave to be considered "done"
        /// </value>
        public bool partOfWave = true;
    }

    /// <summary>
    /// Definition of a single wave in the encounter
    /// </summary>
    [Serializable]
    public class WaveDefinition
    {
        /// <value>
        /// Enemies spawning in the wave
        /// </value>
        public EnemySpawnDefinition[] spawnDefinitions;
    }

    /// <value>
    /// Waves in the encounter
    /// </value>
    public WaveDefinition[] waveDefinitions;

    /// <value>
    /// Objects in the scene of enemies that are already spawned (not prefabs!).
    /// They are considered part of the first wave, meaning they have to be killed for the wave to be considered "done".
    /// </value>
    public GameObject[] firstWavePreSpawnedEnemies;

    /// <value>
    /// Delay from the time enemy is spawned, to the moment it noticed the player.
    /// After <see cref="timeToAlarm"/> seconds, a trigger called "Alarm" is set in the enemy state machine.
    /// Does not affect <see cref="firstWavePreSpawnedEnemies"/>.
    /// </value>
    public float timeToAlarm;

    /// <value>
    /// Border of the camera while the encounter is playing. The camera will never move out of the border.
    /// Note that the player cannot cross the camera border, but enemies can.
    /// </value>
    public Rect cameraBorder;

    /// <value>
    /// Distance from the camera border side where enemy spawns.
    /// For example, if <see cref="EnemySpawnDefinition.direction"/> is <see cref="Rotation.Normal"/>, then the enemy
    /// will spawn in the right point of <see cref="cameraBorder"/>, plus <see cref="spawnSourceDistance"/> in the x Axis.
    /// </value>
    public float spawnSourceDistance = 1;

    /// <value>
    /// Invoked when any wave starts
    /// </value>
    [SerializeField] public ExtEvent onWaveStart;

    /// <value>
    /// Invoked when last wave finishes
    /// </value>
    [SerializeField] public ExtEvent postEncounterEvent;

    private bool stopped;

    /// <summary>
    /// Locks the camera and plays all waves
    /// </summary>
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
        var waveEnemies = index == 0
            ? new List<GameEntity>(firstWavePreSpawnedEnemies.Select(enemy => enemy.GetEntity()))
            : new List<GameEntity>();

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

    private GameEntity SpawnEnemy(EnemySpawnDefinition definition)
    {
        var edge = GetEdge(cameraBorder, definition.direction) + definition.direction * spawnSourceDistance;
        var spawnPoint = new Vector3(edge, 0, definition.z);

        var enemy = GameEntity.Instantiate(definition.prefab, spawnPoint);
        StartTimeout(() => enemy.GetBehaviour<AlarmBrainModule>().SetAlarm(), timeToAlarm);

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

    /// <summary>
    /// Stops the encounter after the current wave is finished. Note that it won't stop a wave while it's running.
    /// </summary>
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