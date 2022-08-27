using UnityEngine;

public class HealthSystem : CharacterBehaviour
{
    public float startHealth;
    [HideInInspector] public float health;

    private void Start()
    {
        health = startHealth;
    }
}
