using UnityEngine;

[RequireComponent(typeof(CachedObject))]
[RequireComponent(typeof(MovableObject))]
public class Character : MonoBehaviour
{
    public PhysicalAttributes physicalAttributes;
    public Animator animator;

    [HideInInspector] public EventManager eventManager;
    [HideInInspector] public MovableObject movableObject;
    // look direction

    private readonly EventListener lookDirectionEvent;
    private int lookDirection = 1;

    public int LookDirection
    {
        get
        {
            return lookDirection;
        }
        set
        {
            lookDirection = value;
            transform.rotation = Quaternion.Euler(0, 90 * lookDirection - 90, 0);
        }
    }

    public void Awake()
    {
        movableObject = GetComponent<MovableObject>();
        eventManager = FindObjectOfType<EventManager>();
    }

    public void OnDestroy()
    {
        eventManager.Detach(lookDirectionEvent);
    }
}
