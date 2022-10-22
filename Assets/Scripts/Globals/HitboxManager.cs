using System.Linq;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    public static HitboxManager Instance { get; private set; }
    [HideInInspector] public Hitbox[] hitboxes;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        hitboxes = new Hitbox[0];
    }

    void Update()
    {
        hitboxes = FindObjectsOfType<Hitbox>();
    }

    public Hitbox[] Barriers => hitboxes.Where(hitbox => hitbox && hitbox.CompareTag("Barrier")).ToArray();

    public Hitbox[] GetOverlapping(Vector3 point)
    {
        return hitboxes.Where(hitbox => hitbox && hitbox.IsInside(point)).ToArray();
    }

    public Hitbox[] GetOverlappingBarriers(Vector3 point)
    {
        return Barriers.Where(hitbox => hitbox.IsInside(point)).ToArray();
    }
}
