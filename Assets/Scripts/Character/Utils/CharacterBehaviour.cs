using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterBehaviour: MonoBehaviour
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
            return character.eventManager;
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
            return character.LookDirection;
        }
        set
        {
            character.LookDirection = value;
        }
    }

    private Character character;

    public virtual void Awake()
    {
        character = GetComponent<Character>();
    }
}