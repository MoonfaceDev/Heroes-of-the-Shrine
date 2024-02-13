public class WalkParticles : CharacterBehaviour
{
    public Particles particles;
    
    protected override void Awake()
    {
        base.Awake();
        var walkBehaviour = GetBehaviour<WalkBehaviour>();
        var jumpBehaviour = GetBehaviour<JumpBehaviour>();

        walkBehaviour.PlayEvents.onPlay += () =>
        {
            if (jumpBehaviour && jumpBehaviour.Playing) return;
            if (!particles.Playing)
            {
                particles.Play();
            }
        };

        walkBehaviour.PlayEvents.onStop += () => particles.Stop();

        if (jumpBehaviour)
        {
            jumpBehaviour.PlayEvents.onPlay += () => particles.Stop();
            jumpBehaviour.PlayEvents.onStop += () =>
            {
                if (walkBehaviour.Playing)
                {
                    particles.Play();
                }
            };
        }
    }
}