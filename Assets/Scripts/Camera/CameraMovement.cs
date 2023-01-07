using UnityEngine;

public class CameraMovement : BaseComponent
{
    public Rect worldBorder;
    [HideInInspector] public Rect border;
    [ShowDebug] public Vector3 targetPosition;
    public float lerpSpeed = 3.0f;

    private Camera mainCamera;

    public float CameraHeight => 2f * mainCamera.orthographicSize;

    public float CameraWidth => CameraHeight * mainCamera.aspect;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        Lock(worldBorder);
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
        if ((next.x - CameraWidth / 2 > border.xMin && next.x + CameraWidth / 2 < border.xMax) ||
            (position.x < next.x && position.x - CameraWidth / 2 < border.xMin) ||
            (position.x > next.x && position.x + CameraWidth / 2 > border.xMax))
        {
            position += (next.x - position.x) * Vector3.right;
        }

        if ((next.y - CameraHeight / 2 > border.yMin && next.y + CameraHeight / 2 < border.yMax) ||
            (position.y < next.y && position.y - CameraHeight / 2 < border.yMin) ||
            (position.y > next.y && position.y + CameraHeight / 2 > border.yMax))
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