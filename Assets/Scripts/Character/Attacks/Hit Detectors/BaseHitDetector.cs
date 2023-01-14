using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Abstract base class for hit detectors, responsible for detecting hits and calling a given function for every hit object
/// </summary>
[Serializable]
public abstract class BaseHitDetector
{
    /// <value>
    /// Attached hitbox
    /// </value>
    public Hitbox hitbox;

    /// <value>
    /// Attached hit particles
    /// </value>
    public HitParticles hitParticles;

    /// <value>
    /// Additional tags of objects this attack can hit.
    /// </value>
    public List<string> includedHittableTags;

    /// <value>
    /// Tags of objects this attack cannot hit.
    /// </value>
    public List<string> excludedHittableTags;

    /// <summary>
    /// Starts detecting hits
    /// </summary>
    /// <param name="hitCallable">Function to be called on detected hit</param>
    /// <param name="hittableTags">Tags of object that can get hit</param>
    public void StartDetector(Action<HittableHitbox> hitCallable, List<string> hittableTags)
    {
        DoStartDetector((hittable) =>
        {
            if (!IsHittable(hittable, hittableTags)) return;

            if (hitParticles && ShouldPlayParticles(hittable))
            {
                var point = hittable.Hitbox.GetIntersectionCenter(hitbox);
                hitParticles.Play(point, hittable);
            }

            hitCallable(hittable);
        });
    }

    private bool IsHittable(IHittable hittable, IEnumerable<string> hittableTags)
    {
        return hittableTags.Concat(includedHittableTags).Except(excludedHittableTags)
            .Any(hittableTag => IsTagIncluded(hittable.Character.tag, hittableTag));
    }

    private static bool IsTagIncluded(string testedTag, string group)
    {
        return testedTag == group || testedTag.StartsWith(group + ".");
    }

    /// <summary>
    /// Abstract method that starts detecting hits. Implementations should include the concrete logic of hit detection.
    /// </summary>
    /// <param name="hitCallable">Function to be called on detected hit</param>
    protected abstract void DoStartDetector(Action<HittableHitbox> hitCallable);

    private static bool ShouldPlayParticles(IHittable hittable)
    {
        return hittable.Character.GetComponent<HittableBehaviour>().CanGetHit();
    }

    /// <summary>
    /// Abstract method that stops detecting hits. Implementations should stop anything that <see cref="DoStartDetector"/> started.
    /// </summary>
    public abstract void StopDetector();
}