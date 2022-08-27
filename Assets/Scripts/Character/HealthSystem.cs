using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float startHealth;
    [HideInInspector] public float health;

    private void Start()
    {
        health = startHealth;
    }
}
