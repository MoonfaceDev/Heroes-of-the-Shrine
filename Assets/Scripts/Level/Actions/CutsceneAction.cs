using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[Serializable]
public class MoveDefinition
{
    public enum Direction
    {
        Left = -1,
        Right = 1,
    }

    public ForcedWalkBehaviour target;
    public Vector3 position;
    public Direction lookDirection = Direction.Right;
}

public class CutsceneAction : MonoBehaviour
{
    public List<MoveDefinition> moveDefinitions;
    public PlayableDirector director;
    public UnityEvent postCutsceneEvent;

    private static readonly float wantedDistance = 0.1f;

    public void Invoke()
    {
        foreach (MoveDefinition definition in moveDefinitions)
        {
            if (!definition.target)
            {
                continue;
            }
            MovableObject movableObject = definition.target.MovableObject;
            definition.target.Play(definition.position);
        }

        EventManager.Instance.Attach(() => moveDefinitions.TrueForAll(definition => definition.target.MovableObject.GroundDistance(definition.position) < wantedDistance), () =>
        {
            moveDefinitions.ForEach(subject => {
                subject.target.Stop();
                subject.target.DisableBehaviours(typeof(CharacterController));
                subject.target.LookDirection = (int)subject.lookDirection;
            });
            director.Play();
            director.stopped += OnStop;
        });
    }

    private void OnStop(PlayableDirector director)
    {
        moveDefinitions.ForEach(subject => {
            subject.target.EnableBehaviours(typeof(CharacterController));
        });
        director.stopped -= OnStop;
        postCutsceneEvent.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (moveDefinitions == null)
        {
            return;
        }
        Gizmos.color = Color.white;
        foreach (var definition in moveDefinitions) {
            Gizmos.DrawWireSphere(MovableObject.ScreenCoordinates(definition.position), 0.2f);
        }
    }
}
