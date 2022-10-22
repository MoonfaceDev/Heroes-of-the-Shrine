using UnityEngine;

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
        this.targetSize = camera.orthographicSize / factor;
        active = true;
    }

    public void Stop()
    {
        active = false;
    }

    private void ZoomCamera()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, position.y, transform.position.z), lerpSpeed * Time.deltaTime);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, lerpSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (active)
        {
            ZoomCamera();
        }
    }
}
