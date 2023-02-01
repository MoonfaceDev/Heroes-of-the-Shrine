using System;
using System.Collections.Generic;
using System.Linq;
using ExtEvents;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Moves entities to predefined points and then plays a cutscene timeline
/// </summary>
public class CutsceneAction : BaseComponent
{
    /// <summary>
    /// Defines how character should move before the cutscene timeline starts
    /// </summary>
    [Serializable]
    public class MoveDefinition
    {
        /// <value>
        /// <see cref="ForcedWalkBehaviour"/> of the character
        /// </value>
        public ForcedWalkBehaviour target;
        
        /// <value>
        /// Speed multiplier while moving
        /// </value>
        public float speedMultiplier = 1;
        
        /// <value>
        /// Point that character is moving towards
        /// </value>
        public Vector3 position;
        
        /// <value>
        /// Character rotation when the timeline starts
        /// </value>
        public Rotation rotation = Rotation.Right;
    }

    /// <value>
    /// Move definitions of the participating characters. Add any character whom position is essential for the timeline.
    /// The <see cref="MoveDefinition"/> will make their start position determinist for the cutscene.
    /// </value>
    public List<MoveDefinition> moveDefinitions;
    
    /// <value>
    /// <seealso cref="PlayableDirector"/> that plays the timeline
    /// </value>
    public PlayableDirector director;
    
    /// <value>
    /// Invoked after timeline is finished
    /// </value>
    [SerializeField] public ExtEvent postCutsceneEvent;
    
    /// <value>
    /// If <c>true</c>, plays the cutscene right when scene is loaded
    /// </value>
    public bool playOnAwake;

    private const float WantedDistance = 0.1f;

    private void Awake()
    {
        if (playOnAwake)
        {
            Invoke();
        }
    }

    /// <summary>
    /// Moves characters according to <see cref="moveDefinitions"/> and plays the cutscene
    /// </summary>
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