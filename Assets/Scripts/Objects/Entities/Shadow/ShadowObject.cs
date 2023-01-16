using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AnimationDefinition
{
    public string animationStateName;
    public float shadowScale;
}

[ExecuteInEditMode]
public class ShadowObject : BaseComponent
{
    [FormerlySerializedAs("movableObject")] public MovableEntity movableEntity;
    public Vector3 shadowScale;
    public Animator figure;
    public AnimationDefinition[] animationDefinitions;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        var scale = 2 / (1 + Mathf.Exp(0.2f * movableEntity.WorldPosition.y));
        foreach (var definition in animationDefinitions)
        {
            if (figure.GetCurrentAnimatorStateInfo(0).IsName(definition.animationStateName))
            {
                scale *= definition.shadowScale;
                break;
            }
        }

        transform.localScale = GameEntity.GroundScreenCoordinates(Vector3.Scale(shadowScale, scale * Vector3.one));
    }
}