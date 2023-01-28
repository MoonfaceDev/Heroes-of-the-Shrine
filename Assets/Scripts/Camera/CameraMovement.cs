using UnityEngine;

/// <summary>
/// Moves the camera smoothly inside a rectangle border
/// </summary>
public class CameraMovement : BaseComponent
{
    /// <value>
    /// Full scene border
    /// </value>
    public Rect worldBorder;

    /// <value>
    /// Current border, affected by encounters 
    /// </value>
    [HideInInspector] public Rect border;

    /// <value>
    /// The wanted position of the camera
    /// </value>
    [ShowDebug] public Vector3 targetPosition;

    /// <value>
    /// The speed at which camera is moving. Higher value is quicker (and less smooth).
    /// </value>
    public float lerpSpeed = 3.0f;

    private Camera mainCamera;
    private float CameraHeight => 2f * mainCamera.orthographicSize;
    private float CameraWidth => CameraHeight * mainCamera.aspect;
    private Vector2 CameraSize => new(CameraWidth, CameraHeight);

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        border = worldBorder;
    }

    /// <summary>
    /// Sets a new camera border
    /// </summary>
    /// <param name="newBorder">New camera border</param>
    public void Lock(Rect newBorder)
    {
        border = newBorder;
    }

    /// <summary>
    /// Changes camera border back to the initial
    /// </summary>
    public void Unlock()
    {
        Lock(worldBorder);
    }

    private void UpdatePosition()
    {
        var position = transform.position;
        var next = Vector3.Lerp(position, targetPosition, lerpSpeed * Time.deltaTime);

        var currentRect = new Rect((Vector2)position - CameraSize / 2, CameraSize);
        var nextRect = new Rect((Vector2)next - CameraSize / 2, CameraSize);

        if ((nextRect.xMin > border.xMin && nextRect.xMax < border.xMax) ||
            (position.x < next.x && currentRect.xMin < border.xMin) ||
            (position.x > next.x && currentRect.xMax > border.xMax))
        {
            position += (next.x - position.x) * Vector3.right;
        }

        if ((nextRect.yMin > border.yMin && nextRect.yMax < border.yMax) ||
            (position.y < next.y && currentRect.yMin < border.yMin) ||
            (position.y > next.y && currentRect.yMax > border.yMax))
        {
            position += (next.y - position.y) * Vector3.up;
        }

        transform.position = position;
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void OnDrawGizmos()
    {
        var rect = border;
#if UNITY_EDITOR
        rect = worldBorder;
#endif
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f),
            new Vector3(rect.size.x, rect.size.y, 0.01f));
    }
}