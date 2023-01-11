using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public abstract class BaseHitDetector : BaseComponent
{
    /// <value>
    /// Additional tags of objects this attack can hit.
    /// </value>
    public List<string> includedHittableTags;

    /// <value>
    /// Tags of objects this attack cannot hit.
    /// </value>
    public List<string> excludedHittableTags;

    protected Hitbox hitbox;

    protected virtual void Awake()
    {
        hitbox = GetComponent<Hitbox>();
    }

    /// <summary>
    /// Checks if a hittable can be hit by this attack.
    /// </summary>
    /// <param name="hittable">Hittable to test.</param>
    /// <param name="hittableTags">Tags of objects this attack can hit.</param>
    /// <returns><c>true</c> if the tag is hittable.</returns>
    private bool IsHittable(IHittable hittable, IEnumerable<string> hittableTags)
    {
        return hittableTags.Concat(includedHittableTags).Except(excludedHittableTags)
            .Any(hittableTag => IsTagIncluded(hittable.Character.tag, hittableTag));
    }

    public void StartDetector(Action<IHittable> hitCallable, List<string> hittableTags)
    {
        DoStartDetector(hittable =>
        {
            if (!IsHittable(hittable, hittableTags)) return;

            var hitParticles = GetComponent<HitParticles>();
            if (hitParticles && ShouldPlayParticles(hittable))
            {
                hitParticles.Play(hittable);
            }

            hitCallable(hittable);
        });
    }

    protected abstract void DoStartDetector(Action<IHittable> hitCallable);

    private static bool ShouldPlayParticles(IHittable hittable)
    {
        return hittable.Character.GetComponent<HittableBehaviour>().CanGetHit();
    }

    public abstract void StopDetector();

    private static bool IsTagIncluded(string testedTag, string group)
    {
        return testedTag == group || testedTag.StartsWith(group + ".");
    }
}