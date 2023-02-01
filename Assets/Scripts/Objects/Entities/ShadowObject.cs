using System;
using UnityEngine;

[ExecuteInEditMode]
public class ShadowObject : BaseComponent
{
    /// <summary>
    /// Mapping between an animator state name to the right shadow scale
    /// </summary>
    [Serializable]
    public class AnimationDefinition
    {
        public string animationStateName;
        public float shadowScale;
    }
    
    /// <value>
    /// Entity that owns the shadow
    /// </value>
    public GameEntity movableEntity;
    
    /// <value>
    /// Initial shadow scale in game coordinates
    /// </value>
    public Vector3 shadowScale;
    
    /// <value>
    /// Related figure to check the animation state - optional
    /// </value>
    public Animator figure;
    
    /// <value>
    /// Animation states that override <see cref="shadowScale"/>
    /// </value>
    public AnimationDefinition[] animationDefinitions;

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