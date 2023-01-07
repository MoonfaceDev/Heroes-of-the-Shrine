using UnityEngine;

[RequireComponent(typeof(CachedObject))]
[RequireComponent(typeof(MovableObject))]
public class Character : BaseComponent
{
    public PhysicalAttributes physicalAttributes;
    public Animator animator;

    [HideInInspector] public MovableObject movableObject;
    [HideInInspector] public AttackManager attackManager;

    public void Awake()
    {
        movableObject = GetComponent<MovableObject>();
        attackManager = GetComponent<AttackManager>();
    }
}