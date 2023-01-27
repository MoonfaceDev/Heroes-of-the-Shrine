using UnityEngine;
using UnityEngine.U2D;

public class CameraFocus : BaseComponent
{
    public float lerpSpeed = 3.0f;

    private Camera mainCamera;
    private bool active;
    private float targetSize;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    public void Zoom(float factor)
    {
        targetSize = mainCamera.orthographicSize / factor;
        active = true;
        GetComponent<PixelPerfectCamera>().enabled = false;
    }

    private void LateUpdate()
    {
        if (!active) return;
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, lerpSpeed * Time.deltaTime);
    }
}