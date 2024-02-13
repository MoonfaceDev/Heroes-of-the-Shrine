using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Zooms the camera smoothly
/// </summary>
public class CameraFocus : BaseComponent
{
    /// <value>
    /// The speed at which camera size is changing. Higher value is quicker (and less smooth).
    /// </value>
    public float lerpSpeed = 3.0f;

    private Camera mainCamera;
    private bool active;
    private float targetSize;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    /// <summary>
    /// Starts zooming
    /// </summary>
    /// <param name="factor">Multiplier by which size is increased</param>
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