using UnityEngine;

public class ShadowObject : MonoBehaviour
{
    public MovableObject movableObject;
    public Vector3 shadowScale;

    // Update is called once per frame
    void Update()
    {
        float scale = 2 / (1 + Mathf.Exp(0.2f * movableObject.position.y));
        transform.localScale = MovableObject.GroundScreenCoordinates(Vector3.Scale(shadowScale, scale * Vector3.one));
    }
}