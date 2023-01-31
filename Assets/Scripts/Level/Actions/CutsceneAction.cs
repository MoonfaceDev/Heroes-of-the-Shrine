using System;
using System.Collections.Generic;
using System.Linq;
using ExtEvents;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

/// <summary>
/// Moves entities to predefined points and then plays a cutscene timeline
/// </summary>
public class CutsceneAction : BaseComponent
{
    [Serializable]
    public class MoveDefinition
    {
        public ForcedWalkBehaviour target;
        public float speedMultiplier = 1;
        public Vector3 position;
        public Rotation rotation = Rotation.Right;
    }

    public List<MoveDefinition> moveDefinitions;
    public PlayableDirector director;
    [SerializeField] public ExtEvent postCutsceneEvent;
    public bool playOnAwake;

    private const float WantedDistance = 0.1f;

    private void Awake()
    {
        if (playOnAwake)
        {
            Invoke();
        }
    }

    public void Invoke()
    {
        var controllers = FindObjectsOfType<CharacterController>();

        foreach (var controller in controllers)
        {
            controller.Enabled = false;
        }

        StartAutoWalk();

        InvokeWhen(
            () => moveDefinitions.TrueForAll(definition =>
                definition.target.MovableEntity.GroundDistance(definition.position) < WantedDistance),
            () =>
            {
                FinishAutoWalk();

                void StoppedCallback(PlayableDirector stoppedDirector)
                {
                    director.stopped -= StoppedCallback;
                    OnCutsceneFinish(controllers);
                }

                director.stopped += StoppedCallback;
                director.Play();
            }
        );
    }

    private void StartAutoWalk()
    {
        foreach (var definition in moveDefinitions.Where(definition => definition.target))
        {
            definition.target.GetComponent<WalkBehaviour>().speed *= definition.speedMultiplier;
            definition.target.Play(new ForcedWalkBehaviour.Command(definition.position, WantedDistance));
        }
    }

    private void FinishAutoWalk()
    {
        moveDefinitions.ForEach(definition =>
        {
            definition.target.Stop();
            definition.target.GetComponent<WalkBehaviour>().speed /= definition.speedMultiplier;
            definition.target.MovableEntity.rotation = definition.rotation;
        });
    }

    private void OnCutsceneFinish(IEnumerable<CharacterController> disabledControllers)
    {
        foreach (var controller in disabledControllers)
        {
            if (controller)
            {
                controller.Enabled = true;
            }
        }

        postCutsceneEvent.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        foreach (var definition in moveDefinitions)
        {
            Gizmos.DrawWireSphere(GameEntity.ScreenCoordinates(definition.position), 0.2f);
        }
    }
}