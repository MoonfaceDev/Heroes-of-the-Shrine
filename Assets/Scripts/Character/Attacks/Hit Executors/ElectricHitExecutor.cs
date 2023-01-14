using System;
using Random = UnityEngine.Random;

[Serializable]
public class ElectricHitExecutor : IHitExecutor
{
    [Serializable]
    public class ElectrifyEffectDefinition
    {
        public float duration;
        public float speedMultiplier;
    }
    
    public SimpleHitExecutor simpleHitExecutor;
    public float electrifyRate;
    public ElectrifyEffectDefinition electrifyEffect;

    public void Execute(BaseAttack attack, IHittable hittable)
    {
        simpleHitExecutor.Execute(attack, hittable);
        if (!(Random.Range(0f, 1f) < electrifyRate)) return;
        var electrifiedEffect = hittable.Character.GetComponent<ElectrifiedEffect>();
        if (electrifiedEffect)
        {
            electrifiedEffect.Play(new ElectrifiedEffectCommand(electrifyEffect.duration,
                electrifyEffect.speedMultiplier));
        }
    }
}