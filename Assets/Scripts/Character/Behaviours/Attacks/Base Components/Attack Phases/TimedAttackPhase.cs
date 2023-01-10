using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class TimedAttackPhase : IAttackPhase
{
    /// <value>
    /// Duration of the phase, in seconds
    /// </value>
    public float duration;

    public IEnumerator Play()
    {
        yield return new WaitForSeconds(duration);
    }
}