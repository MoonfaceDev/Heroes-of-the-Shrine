using UnityEngine;

public class HealthSystem : CharacterBehaviour
{
    public float startHealth;
    [HideInInspector] public float health;

    public float Fraction => health / startHealth;

    public override void Awake()
    {
        base.Awake();
        health = startHealth;
    }
}
