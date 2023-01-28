using UnityEngine;

public class HealthSystem : CharacterBehaviour
{
    public float startHealth;
    [HideInInspector] public float health;

    public float Fraction => health / startHealth;

    protected override void Awake()
    {
        base.Awake();
        health = startHealth;
    }

    public bool Alive => health > 0;
}