using UnityEngine;

public class UncontrolledBehaviour : PlayableBehaviour
{
    private bool active;

    public override bool Playing => active;

    public void Play()
    {
        if (!CanPlay())
        {
            return;
        }

        DisableBehaviours(typeof(CharacterBehaviour));
        Enabled = true;
        StopBehaviours(typeof(PlayableBehaviour));

        active = true;
        InvokeOnPlay();
    }

    public override void Stop()
    {
        if (active)
        {
            InvokeOnStop();
            active = false;
            EnableBehaviours(typeof(CharacterBehaviour));
        }
    }
}
