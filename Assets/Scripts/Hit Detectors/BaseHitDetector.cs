using System;
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
    public Tags includedHittableTags;

    /// <value>
    /// Tags of objects this attack cannot hit.
    /// </value>
    public Tags excludedHittableTags;

    /// <summary>
    /// Starts detecting hits
    /// </summary>
    /// <param name="hitCallable">Function to be called on detected hit</param>
    /// <param name="hittableTags">Tags of object that can get hit</param>
    public void StartDetector(Action<HittableHitbox> hitCallable, Tags hittableTags)
    {
        DoStartDetector(hittable =>
        {
            if (!(IsHittable(hittable, hittableTags) && hittable.CanGetHit())) return;

            if (hitParticles)
            {
                var point = hittable.Hitbox.GetIntersectionCenter(hitbox);
                hitParticles.Play(point, hittable);
            }

            hitCallable(hittable);
        });
    }

    private bool IsHittable(IHittable hittable, Tags hittableTags)
    {
        return hittableTags.Concat(includedHittableTags).Except(excludedHittableTags)
            .Intersect(hittable.Character.movableEntity.tags).Any();
    }

    /// <summary>
    /// Abstract method that starts detecting hits. Implementations should include the concrete logic of hit detection.
    /// </summary>
    /// <param name="hitCallable">Function to be called on detected hit</param>
    protected abstract void DoStartDetector(Action<HittableHitbox> hitCallable);

    /// <summary>
    /// Abstract method that stops detecting hits. Implementations should stop anything that <see cref="DoStartDetector"/> started.
    /// </summary>
    public abstract void StopDetector();
}