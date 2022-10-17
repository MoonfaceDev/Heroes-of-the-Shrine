using UnityEngine;

public class ShadowObject : MonoBehaviour
{
    public MovableObject movableObject;
    public Vector3 shadowScale;
    public KnockbackBehaviour knockbackBehaviour;
    public float recoveringFromKnockbackScale;

    // Update is called once per frame
    void Update()
    {
        float scale = 2 / (1 + Mathf.Exp(0.2f * movableObject.position.y));
        if (knockbackBehaviour && knockbackBehaviour.Recovering)
        {
            scale *= recoveringFromKnockbackScale;
        }
        transform.localScale = MovableObject.GroundScreenCoordinates(Vector3.Scale(shadowScale, scale * Vector3.one));
    }
}