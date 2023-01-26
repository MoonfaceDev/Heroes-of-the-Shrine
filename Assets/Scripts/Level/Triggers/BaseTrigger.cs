using ExtEvents;
using UnityEngine;

public abstract class BaseTrigger : BaseComponent
{
    [SerializeField] public ExtEvent action;
}