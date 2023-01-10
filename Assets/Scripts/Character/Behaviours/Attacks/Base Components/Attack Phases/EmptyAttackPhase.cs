using System;
using System.Collections;

[Serializable]
public class EmptyAttackPhase : IAttackPhase
{
    public IEnumerator Play()
    {
        yield break;
    }
}