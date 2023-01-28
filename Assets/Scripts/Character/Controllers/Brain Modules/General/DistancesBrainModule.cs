using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// State machine parameters based on distances between the enemy, to any defined point in the scene
/// </summary>
public class DistancesBrainModule : BrainModule
{
    /// <summary>
    /// Point to measure distance from and update related parameters
    /// </summary>
    [Serializable]
    public class DistanceParameterEntry
    {
        /// <value>
        /// Name of the parameter in the state machine animator
        /// </value>
        public string parameterName;
        
        /// <value>
        /// Coordinates of the point on the ground, translated to (x, 0, y) in world coordinates
        /// </value>
        public Vector2 groundPoint;
    }

    /// <value>
    /// List of points parameters
    /// </value>
    public List<DistanceParameterEntry> distanceAnimatorParameters;

    protected override void Update()
    {
        base.Update();
        foreach (var entry in distanceAnimatorParameters)
        {
            StateMachine.SetFloat(entry.parameterName,
                MovableEntity.GroundDistance(MathUtils.ToSpace(entry.groundPoint)));
        }
    }

    public override string[] GetParameters()
    {
        return distanceAnimatorParameters.Select(entry => entry.parameterName).ToArray();
    }
}