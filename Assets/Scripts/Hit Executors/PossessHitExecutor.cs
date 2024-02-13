using System;

/// <summary>
/// Hit executor that applies possesses effect to an hittable
/// </summary>
[Serializable]
public class PossessHitExecutor : IHitExecutor
{
    /// <value>
    /// Effect duration
    /// </value>
    public float duration;

    public void Execute(Hit hit)
    {
        var possessedEffect = hit.Victim.RelatedEntity.GetBehaviour<PossessedEffect>();
        if (possessedEffect)
        {
            possessedEffect.Play(new PossessedEffect.Command { maxDuration = duration });
        }
    }
}