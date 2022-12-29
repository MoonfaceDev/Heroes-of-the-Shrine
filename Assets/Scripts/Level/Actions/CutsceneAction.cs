using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[Serializable]
public class MoveDefinition
{
    public enum Direction
    {
        Left = -1,
        Right = 1
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

    private const float WantedDistance = 0.5f;

    public void Invoke()
    {
        foreach (var definition in moveDefinitions.Where(definition => definition.target))
        {
            definition.target.Play(definition.position);
        }

        EventManager.Instance.Attach(
            () => moveDefinitions.TrueForAll(definition =>
                definition.target.MovableObject.GroundDistance(definition.position) < WantedDistance), () =>
            {
                moveDefinitions.ForEach(subject =>
                {
                    subject.target.Stop();
                    subject.target.MovableObject.rotation = (int)subject.lookDirection;
                });
                foreach (var controller in FindObjectsOfType<CharacterController>())
                {
                    controller.Enabled = false;
                }

                if (director)
                {
                    director.Play();
                    director.stopped += OnStop;
                }
                else
                {
                    OnStop(director);
                }
            });
    }

    private void OnStop(PlayableDirector stoppedDirector)
    {
        if (stoppedDirector)
        {
            stoppedDirector.stopped -= OnStop;
        }

        foreach (var controller in FindObjectsOfType<CharacterController>())
        {
            controller.Enabled = true;
        }

        postCutsceneEvent.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (moveDefinitions == null)
        {
            return;
        }

        Gizmos.color = Color.white;
        foreach (var definition in moveDefinitions)
        {
            Gizmos.DrawWireSphere(MovableObject.ScreenCoordinates(definition.position), 0.2f);
        }
    }
}