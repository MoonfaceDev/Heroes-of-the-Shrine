using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DistancesBrainModule : BrainModule
{
    [Serializable]
    public class DistanceParameterEntry
    {
        public string parameterName;
        public Vector2 groundPoint;
    }


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