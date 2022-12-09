using UnityEngine;

[RequireComponent(typeof(CachedObject))]
[RequireComponent(typeof(MovableObject))]
public class Character : MonoBehaviour
{
    public PhysicalAttributes physicalAttributes;
    public Animator animator;

    [HideInInspector] public EventManager eventManager;
    [HideInInspector] public MovableObject movableObject;

    public void Awake()
    {
        movableObject = GetComponent<MovableObject>();
        eventManager = FindObjectOfType<EventManager>();
    }
}
