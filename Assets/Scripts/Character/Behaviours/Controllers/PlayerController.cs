﻿using System;
using System.Collections.Generic;
using System.Linq;
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

public struct BufferedAction
{
    public Func<bool> action;
    public int priority;
    public float insertionTime;
}

public class PlayerController : CharacterController
{
    public AttackProperty[] attacks;
    public RuntimeAnimatorController[] animatorControllers;

    [HideInInspector]
    public int activeSuitIndex = 0;

    public float bufferingTime;
    public BaseAttack[] nonBufferedAttacks;

    [Header("Buffered actions priorities")]
    public int jumpPriority;
    public int slidePriority;
    public int attackPriority;

    private WalkBehaviour walkBehaviour;
    private JumpBehaviour jumpBehaviour;
    private SlideBehaviour slideBehaviour;
    private DodgeBehaviour dodgeBehaviour;

    private List<BufferedAction> bufferedActions;

    public override void Awake()
    {
        base.Awake();
        walkBehaviour = GetComponent<WalkBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        slideBehaviour = GetComponent<SlideBehaviour>();
        dodgeBehaviour = GetComponent<DodgeBehaviour>();

        bufferedActions = new();
    }

    private void Start()
    {
        // clear expired buffered actions
        EventManager.Attach(() => true, () => {
            bufferedActions = bufferedActions.FindAll(action => Time.time < action.insertionTime + bufferingTime);
        }, false);
    }

    public void Update()
    {
        // play the buffered action with highest priority
        if (bufferedActions.Count > 0)
        {
            foreach(BufferedAction bufferedAction in bufferedActions.OrderByDescending(action => action.priority))
            {
                bool isSuccess = bufferedAction.action();
                if (isSuccess)
                {
                    bufferedActions.Clear();
                    break;
                }
            }
        }

        int horizontal = Direction(Input.GetAxisRaw("Horizontal"));
        int vertical = Direction(Input.GetAxisRaw("Vertical"));
        if (walkBehaviour)
        {
            walkBehaviour.Play(horizontal, vertical);
        }
        //jumping
        ExecuteJump();
        //sliding
        ExecuteSlide();
        //dodging
        ExecuteDodge();
        //attacks
        ExecuteAttack();
        //change suits
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            activeSuitIndex = 1 - activeSuitIndex;
            Animator.runtimeAnimatorController = animatorControllers[activeSuitIndex];
        }
    }

    private void ExecuteJump()
    {
        if (jumpBehaviour && Input.GetButtonDown(Button.Jump.ToString())) //pressed jump
        {
            ExecuteAction(jumpBehaviour.Play, jumpBehaviour.CanPlay, jumpPriority);
        }
    }

    private void ExecuteSlide()
    {
        if (slideBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed slide
        {
            ExecuteAction(slideBehaviour.Play, slideBehaviour.CanPlay, slidePriority);
        }
    }

    private void ExecuteDodge()
    {
        int vertical = Direction(Input.GetAxisRaw("Vertical"));
        if (dodgeBehaviour && Input.GetButtonDown(Button.Escape.ToString())) //pressed dodge
        {
            if (vertical != 0)
            {
                dodgeBehaviour.Play(vertical);
            }
        }
    }

    private bool ExecuteAttack(bool isBuffered = false, Button[] bufferedButtons = null)
    {
        AttackProperty selectedAttack = null;
        foreach (AttackProperty property in attacks)
        {
            if (property.attack.CanPlay())
            {
                if (isBuffered && bufferedButtons != null && bufferedButtons.Contains(property.button))
                {
                    selectedAttack = property;
                }
                if (!isBuffered && Input.GetButtonDown(property.button.ToString()))
                {
                    selectedAttack = property;
                }
            }
        }
        if (selectedAttack != null && !(isBuffered && nonBufferedAttacks.Contains(selectedAttack.attack)))
        {
            selectedAttack.attack.Play();
            Debug.Log("Started attack " + selectedAttack.attack.AttackName);
            return true;
        }
        else if (!isBuffered)
        {
            Button[] downButtons = GetDownButtons();
            if (downButtons.Length > 0)
            {
                bufferedActions.Add(new BufferedAction { action = () => ExecuteAttack(true, downButtons), priority = attackPriority, insertionTime = Time.time });
            }
        }
        return false;
    }

    private void ExecuteAction(Action play, Func<bool> canPlay, int bufferingPriority)
    {
        if (canPlay())
        {
            play();
        }
        else
        {
            bufferedActions.Add(new BufferedAction
            {
                action = () => {
                    if (canPlay())
                    {
                        play();
                        return true;
                    }
                    return false;
                },
                priority = bufferingPriority,
                insertionTime = Time.time
            });
        }
    }

    private Button[] GetDownButtons()
    {
        return Enum.GetValues(typeof(Button)).Cast<Button>().Where(button => Input.GetButtonDown(button.ToString())).ToArray();
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
