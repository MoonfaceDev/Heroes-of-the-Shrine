﻿using System;

/// <summary>
/// Hit executor that stuns an hittable
/// </summary>
[Serializable]
public class StunHitExecutor : IHitExecutor
{
    /// <value>
    /// Duration of stun effect caused by hit
    /// </value>
    public float stunTime = 0.5f;

    private const float StunLaunchPower = 1;
    private const float StunLaunchAngle = 90; // degrees

    public void Execute(Hit hit)
    {
        var knockbackBehaviour = hit.Victim.RelatedEntity.GetBehaviour<KnockbackBehaviour>();
        var stunBehaviour = hit.Victim.RelatedEntity.GetBehaviour<StunBehaviour>();

        if (knockbackBehaviour && hit.Victim.RelatedEntity.WorldPosition.y > 0)
        {
            knockbackBehaviour.Play(new KnockbackBehaviour.Command
                { power = StunLaunchPower, angleDegrees = StunLaunchAngle }
            );
            return;
        }

        if (stunBehaviour)
        {
            stunBehaviour.Play(new StunBehaviour.Command { time = stunTime });
        }
    }
}