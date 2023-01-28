using UnityEngine;

public class CameraMovement : BaseComponent
{
    public Rect worldBorder;
    [HideInInspector] public Rect border;
    [ShowDebug] public Vector3 targetPosition;
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

    public void Lock(Rect newBorder)
    {
        border = newBorder;
    }

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