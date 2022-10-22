using UnityEngine;

public class UncontrolledBehaviour : PlayableBehaviour
{
    public bool Active
    {
        get => active;
        private set
        {
            active = value;
            Animator.SetBool("uncontrolled", active);
        }
    }

    private bool active;

    public override bool Playing => Active;

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }

        DisableBehaviours(typeof(CharacterBehaviour));
        Enabled = true;
        StopBehaviours(typeof(PlayableBehaviour));

        Active = true;
        InvokeOnPlay();
    }

    public override void Stop()
    {
        if (Active)
        {
            InvokeOnStop();
            Active = false;
            EnableBehaviours(typeof(CharacterBehaviour));
        }
    }
}
