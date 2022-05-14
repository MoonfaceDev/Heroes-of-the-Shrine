using System.Collections;
using UnityEngine;

public interface ICharacterMovement
{
    public void Knockback(float direction, float distance, float height);

    public void Stun(float time);
}