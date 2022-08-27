using System;
using System.Collections;
using UnityEngine;

public class PlayerController : CharacterBehaviour
{
    public RuntimeAnimatorController[] controllers;

    [HideInInspector]
    public int activeSuitIndex = 0;

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private SlideBehaviour slideBehaviour;

    private void Start()
    {
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
    }

    public void Update()
    {
        float horizontal = Direction(Input.GetAxisRaw("Horizontal"));
        float vertical = Direction(Input.GetAxisRaw("Vertical"));
        walkBehaviour.Walk(horizontal, vertical);
        //jumping
        if (jumpBehaviour && Input.GetButtonDown("Jump")) //pressed jump
        {
            jumpBehaviour.Jump();
        }
        //sliding
        if (slideBehaviour && Input.GetButtonDown("Slide")) //pressed slide
        {
            slideBehaviour.Slide();
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
