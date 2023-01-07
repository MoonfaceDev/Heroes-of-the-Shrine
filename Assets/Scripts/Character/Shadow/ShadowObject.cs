using System;
using UnityEngine;

[Serializable]
public class AnimationDefinition
{
    public string animationStateName;
    public float shadowScale;
}

[ExecuteInEditMode]
public class ShadowObject : BaseComponent
{
    public MovableObject movableObject;
    public Vector3 shadowScale;
    public Animator figure;
    public AnimationDefinition[] animationDefinitions;

    // Update is called once per frame
    private void Update()
    {
        var scale = 2 / (1 + Mathf.Exp(0.2f * movableObject.WorldPosition.y));
        foreach (var definition in animationDefinitions)
        {
            if (figure.GetCurrentAnimatorStateInfo(0).IsName(definition.animationStateName))
            {
                scale *= definition.shadowScale;
                break;
            }
        }

        transform.localScale = MovableObject.GroundScreenCoordinates(Vector3.Scale(shadowScale, scale * Vector3.one));
    }
}