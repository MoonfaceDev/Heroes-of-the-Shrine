using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    public float duration;

    private float cooldownStartTime = float.MinValue;

    public void Reset()
    {
        cooldownStartTime = Time.time;
    }

    public bool CanPlay()
    {
        return duration <= 0 || Time.time - cooldownStartTime > duration;
    }
}