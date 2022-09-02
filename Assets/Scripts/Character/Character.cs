using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class Character : MonoBehaviour
{
    public Animator animator;

    [HideInInspector] public EventManager eventManager;
    [HideInInspector] public MovableObject movableObject;
    // look direction
    [HideInInspector] public int lookDirection = 1;

    private KnockbackBehaviour knockbackBehaviour;
    private EventListener lookDirectionEvent;

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
        //Update look direction
        knockbackBehaviour = GetComponent<KnockbackBehaviour>();
        lookDirectionEvent = eventManager.Attach(
            () => Mathf.Abs(movableObject.velocity.x) > Mathf.Epsilon,
            () =>
            {
                if (knockbackBehaviour && knockbackBehaviour.active)
                {
                    LookDirection = -Mathf.RoundToInt(Mathf.Sign(movableObject.velocity.x));
                }
                else
                {
                    LookDirection = Mathf.RoundToInt(Mathf.Sign(movableObject.velocity.x));
                }
            },
            single: false
        );
    }

    public void OnDestroy()
    {
        eventManager.Detach(lookDirectionEvent);
    }
}
