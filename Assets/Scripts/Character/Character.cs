using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class Character : MonoBehaviour
{
    public Animator animator;

    [HideInInspector] public EventManager eventManager;
    [HideInInspector] public MovableObject movableObject;
    // look direction
    [HideInInspector] public int lookDirection = 1;

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

    void Start()
    {
        movableObject = GetComponent<MovableObject>();
        eventManager = FindObjectOfType<EventManager>();
        //Update look direction
        LookDirectionEvent lookDirectionEvent = new(this);
        eventManager.AttachEvent(lookDirectionEvent);
    }

    class LookDirectionEvent : IEventListener
    {
        private readonly Character character;
        private KnockbackBehaviour knockbackBehaviour;

        public LookDirectionEvent(Character character)
        {
            this.character = character;
            knockbackBehaviour = character.GetComponent<KnockbackBehaviour>();
        }

        public void Callback()
        {
            if (knockbackBehaviour && knockbackBehaviour.knockback)
            {
                character.LookDirection = -Mathf.RoundToInt(Mathf.Sign(character.movableObject.velocity.x));
            }
            else
            {
                character.LookDirection = Mathf.RoundToInt(Mathf.Sign(character.movableObject.velocity.x));
            }
        }

        public bool Validate()
        {
            return Mathf.Abs(character.movableObject.velocity.x) > Mathf.Epsilon;
        }
    }
}
