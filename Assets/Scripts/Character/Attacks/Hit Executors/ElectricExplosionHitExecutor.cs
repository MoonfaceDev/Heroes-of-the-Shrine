using System;
using Random = UnityEngine.Random;

[Serializable]
public class ElectricExplosionHitExecutor : IHitExecutor
{
    [Serializable]
    public class ElectrifyEffectDefinition
    {
        public float duration;
        public float speedMultiplier;
    }

    public SimpleHitExecutor simpleHitExecutor;
    public ElectrifyEffectDefinition electrifyEffect;

    public void Execute(BaseAttack attack, IHittable hittable)
    {
        simpleHitExecutor.Execute(attack, hittable);
        var electrifiedEffect = hittable.Character.GetComponent<ElectrifiedEffect>();
        if (electrifiedEffect)
        {
            electrifiedEffect.Play(new ElectrifiedEffect.Command(electrifyEffect.duration,
                electrifyEffect.speedMultiplier));
        }
    }
}