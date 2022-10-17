using UnityEngine;

public class HealthSystem : CharacterBehaviour
{
    public float startHealth;
    [HideInInspector] public float health;

    public override void Awake()
    {
        base.Awake();
        health = startHealth;
    }
}
