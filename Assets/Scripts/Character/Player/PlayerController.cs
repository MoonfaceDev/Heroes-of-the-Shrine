using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public SimpleCharacterMovement movement;
    public Animator animator;
    public RuntimeAnimatorController[] controllers;

    [HideInInspector]
    public int activeSuitIndex = 0;

    public void Update()
    {
        float horizontal = Direction(Input.GetAxisRaw("Horizontal"));
        float vertical = Direction(Input.GetAxisRaw("Vertical"));
        movement.Walk(horizontal, vertical);
        //jumping
        if (Input.GetButtonDown("Jump")) //pressed jump
        {
            movement.Jump();
        }
        //sliding
        if (Input.GetButtonDown("Slide")) //pressed slide
        {
            movement.Slide();
        }
        //change suits
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            activeSuitIndex = 1 - activeSuitIndex;
            animator.runtimeAnimatorController = controllers[activeSuitIndex];
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
