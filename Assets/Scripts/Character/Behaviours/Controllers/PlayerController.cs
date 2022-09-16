﻿using System;
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

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
    }

    public void Update()
    {
        float horizontal = Direction(Input.GetAxisRaw("Horizontal"));
        float vertical = Direction(Input.GetAxisRaw("Vertical"));
        if (walkBehaviour)
        {
            walkBehaviour.Walk(horizontal, vertical);
        }
        //jumping
        if (jumpBehaviour && Input.GetButtonDown(Button.Jump.ToString())) //pressed jump
        {
            jumpBehaviour.Jump();
        }
        //sliding
        if (slideBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed slide
        {
            slideBehaviour.Slide();
        }
        //attacks
        foreach (AttackProperty property in attacks)
        {
            if (property.attack && Input.GetButtonDown(property.button.ToString()))
            {
                try
                {
                    property.attack.Attack();
                    Debug.Log("Started attack " + property.attack.attackName);
                    return;
                }
                catch (CannotAttackException) { }
            }
        }
        //change suits
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            activeSuitIndex = 1 - activeSuitIndex;
            animator.runtimeAnimatorController = animatorControllers[activeSuitIndex];
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
