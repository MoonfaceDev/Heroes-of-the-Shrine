using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBehaviour
{
    public RuntimeAnimatorController[] animatorControllers;

    [HideInInspector]
    public int activeSuitIndex = 0;

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private SlideBehaviour slideBehaviour;
    private RunKick runKick;
    private Dictionary<BaseAttack, string> attacks;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
        attacks = new();
        attacks.Add(GetComponent<RunKick>(), "Attack");
        attacks.Add(GetComponent<NormalAttack>(), "Attack");
        attacks.Add(GetComponent<AltNormalAttack>(), "Attack");
        attacks.Add(GetComponent<Uppercut>(), "Attack");
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
        if (jumpBehaviour && Input.GetButtonDown("Jump")) //pressed jump
        {
            jumpBehaviour.Jump();
        }
        //sliding
        if (slideBehaviour && Input.GetButtonDown("Escape")) //pressed slide
        {
            slideBehaviour.Slide();
        }
        //attacks
        foreach (KeyValuePair<BaseAttack, string> attack in attacks)
        {
            if (attack.Key && Input.GetButtonDown(attack.Value))
            {
                attack.Key.Attack();
                return;
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
