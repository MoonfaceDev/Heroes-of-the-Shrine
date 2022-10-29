using UnityEngine;
using UnityEngine.U2D;

public class CameraFocus : MonoBehaviour
{
    public float lerpSpeed = 3.0f;

    private new Camera camera;
    private bool active;
    private Vector2 position;
    private float targetSize;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    public void Zoom(Vector2 position, float factor)
    {
        this.position = position;
        targetSize = camera.orthographicSize / factor;
        active = true;
        GetComponent<PixelPerfectCamera>().enabled = false;
    }

    public void Stop()
    {
        active = false;
        GetComponent<PixelPerfectCamera>().enabled = true;
    }

    private void LateUpdate()
    {
        if (active)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, position.y, transform.position.z), lerpSpeed * Time.deltaTime);
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, lerpSpeed * Time.deltaTime);
        }
    }
}
