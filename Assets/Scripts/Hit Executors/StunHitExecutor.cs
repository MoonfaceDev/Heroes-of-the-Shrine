using System;

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

    public void Execute(BaseAttack attack, IHittable hittable)
    {
        hittable.Stun(stunTime);
    }
}