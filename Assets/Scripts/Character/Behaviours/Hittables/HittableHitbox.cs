using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class HittableHitbox : MonoBehaviour, IHittable
{
    public HittableBehaviour hittableBehaviour;
    
    [Header("Blink Effect")]
    public SpriteRenderer figure;
    public Material blinkMaterial;
    public float blinkTime;

    public Hitbox Hitbox { get; private set; }
    
    private Material defaultMaterial;
    private EventListener blinkEvent;

    protected virtual void Awake()
    {
        Hitbox = GetComponent<Hitbox>();
        defaultMaterial = figure.material;
    }

    public Character Character => hittableBehaviour.Character;
    
    protected void Blink()
    {
        if (!figure || !hittableBehaviour.CanGetHit()) return;
        
        figure.material = blinkMaterial;
        blinkEvent = EventManager.Instance.StartTimeout(() => figure.material = defaultMaterial, blinkTime);
    }

    public virtual void Hit(float damage)
    {
        Blink();
        hittableBehaviour.Hit(damage);
    }

    public virtual void Knockback(float damage, float power, float angleDegrees, float stunTime)
    {
        if (figure && hittableBehaviour.CanGetHit())
        {
            Blink();
        }
        hittableBehaviour.Knockback(damage, power, angleDegrees, stunTime);
    }

    public virtual void Stun(float damage, float time)
    {
        if (figure && hittableBehaviour.CanGetHit())
        {
            Blink();
        }
        hittableBehaviour.Stun(damage, time);
    }
    
    private void OnDestroy()
    {
        EventManager.Instance.Detach(blinkEvent);
    }
}