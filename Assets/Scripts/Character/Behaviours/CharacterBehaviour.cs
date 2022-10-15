using UnityEngine;

[RequireComponent(typeof(Character))]
public abstract class CharacterBehaviour: MonoBehaviour
{
    public static float gravityAcceleration = 20;
    public Animator animator
    {
        get
        {
            return character.animator;
        }
    }
    public EventManager eventManager
    {
        get
        {
            return FindObjectOfType<EventManager>();
        }
    }
    public MovableObject movableObject
    {
        get
        {
            return character.movableObject;
        }
    }
    public int lookDirection
    {
        get
        {
            return character.lookDirection;
        }
        set
        {
            character.lookDirection = value;
        }
    }

    private Character character;

    public virtual void Awake()
    {
        character = GetComponent<Character>();
    }

    public abstract void Stop();
}