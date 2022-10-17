using System;
using UnityEngine;

public enum Button
{
    Jump,
    Escape,
    Attack,
    Defense
}

[Serializable]
public class AttackProperty
{
    public BaseAttack attack;
    public Button button;
}

public class PlayerController : CharacterBehaviour
{
    public AttackProperty[] attacks;
    public RuntimeAnimatorController[] animatorControllers;

    [HideInInspector]
    public int activeSuitIndex = 0;

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private SlideBehaviour slideBehaviour;
    private DodgeBehaviour dodgeBehaviour;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
        dodgeBehaviour = GetComponent<DodgeBehaviour>();
    }

    public void Update()
    {
        float horizontal = Direction(Input.GetAxisRaw("Horizontal"));
        float vertical = Direction(Input.GetAxisRaw("Vertical"));
        if (walkBehaviour)
        {
            walkBehaviour.Play(horizontal, vertical);
        }
        //jumping
        if (jumpBehaviour && Input.GetButtonDown(Button.Jump.ToString())) //pressed jump
        {
            jumpBehaviour.Play();
        }
        //sliding
        if (slideBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed slide
        {
            slideBehaviour.Play();
        }
        //dodging
        if (dodgeBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed dodge
        {
            dodgeBehaviour.Play();
        }
        //attacks
        AttackProperty selectedAttack = null;
        foreach (AttackProperty property in attacks)
        {
            if (Input.GetButtonDown(property.button.ToString()) && property.attack.CanPlay())
            {
                selectedAttack = property;
            }
        }
        if (selectedAttack != null)
        {
            selectedAttack.attack.Play();
            Debug.Log("Started attack " + selectedAttack.attack.AttackName);
            return;
        }
        //change suits
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            activeSuitIndex = 1 - activeSuitIndex;
            Animator.runtimeAnimatorController = animatorControllers[activeSuitIndex];
        }
    }

    public int Direction(float number)
    {
        if (number > Mathf.Epsilon)
        {
            return 1;
        }
        if (number < -Mathf.Epsilon)
        {
            return -1;
        }
        return 0;
    }
}
