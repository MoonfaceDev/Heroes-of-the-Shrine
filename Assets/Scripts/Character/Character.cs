using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class Character : MonoBehaviour
{
    public Animator animator;

    [HideInInspector] public EventManager eventManager;
    [HideInInspector] public MovableObject movableObject;
    // look direction

    private readonly EventListener lookDirectionEvent;
    private int _lookDirection = 1;

    public int LookDirection
    {
        get
        {
            return _lookDirection;
        }
        set
        {
            _lookDirection = value;
            transform.rotation = Quaternion.Euler(0, 90 * _lookDirection - 90, 0);
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
