using UnityEngine;
using UnityEngine.U2D;

public class CameraFocus : BaseComponent
{
    public float lerpSpeed = 3.0f;

    private Camera mainCamera;
    private bool active;
    private Vector2 currentPosition;
    private float targetSize;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    public void Zoom(Vector2 position, float factor)
    {
        currentPosition = position;
        targetSize = mainCamera.orthographicSize / factor;
        active = true;
        GetComponent<PixelPerfectCamera>().enabled = false;
    }

    private void LateUpdate()
    {
        if (!active) return;
        var position = transform.position;
        position = Vector3.Lerp(position, new Vector3(currentPosition.x, currentPosition.y, position.z),
            lerpSpeed * Time.deltaTime);
        transform.position = position;
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, lerpSpeed * Time.deltaTime);
    }
}