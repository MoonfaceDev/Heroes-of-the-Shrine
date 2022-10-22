using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public string targetTag;
    public Rect worldBorder;
    [HideInInspector] public Rect border;
    public float lerpSpeed = 3.0f;

    private new Camera camera;
    private MovableObject target;
    private Vector3 offset;

    public float CameraHeight => 2f * camera.orthographicSize;

    public float CameraWidth => CameraHeight * camera.aspect;

    public Vector2 CameraSize => new(CameraWidth, CameraHeight);

    private void Awake()
    {
        camera = GetComponent<Camera>();
        target = GameObject.FindGameObjectWithTag(targetTag).GetComponent<MovableObject>();
    }

    private void Start()
    {
        Lock(worldBorder);
        if (target)
        {
            offset = transform.position - MovableObject.GroundScreenCoordinates(target.position);
        }
    }

    public void Lock(Rect newBorder)
    {
        border = newBorder;
    }

    public void Lock(float leftBorder, float rightBorder)
    {
        Rect newBorder = border;
        newBorder.xMin = leftBorder;
        newBorder.xMax = rightBorder;
        Lock(newBorder);
    }

    public void Unlock()
    {
        Lock(worldBorder);
    }

    private void MoveCamera(Vector3 targetPos)
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 targetPosition = MovableObject.GroundScreenCoordinates(target.position) + offset;
        if ((transform.position.x - CameraWidth / 2 > border.xMin && transform.position.x + CameraWidth / 2 < border.xMax) || (transform.position.x < targetPosition.x && transform.position.x - CameraWidth / 2 < border.xMin) || (transform.position.x > targetPosition.x && transform.position.x + CameraWidth / 2 > border.xMax))
        {
            MoveCamera(transform.position + (targetPosition.x - transform.position.x) * Vector3.right);
        }
        if ((transform.position.y - CameraHeight / 2 > border.yMin && transform.position.y + CameraHeight / 2 < border.yMax) || (transform.position.y < targetPosition.y && transform.position.y - CameraHeight / 2 < border.yMin) || (transform.position.y > targetPosition.y && transform.position.y + CameraHeight / 2 > border.yMax))
        {
            MoveCamera(transform.position + (targetPosition.y - transform.position.y) * Vector3.up);
        }
    }

    void OnDrawGizmos()
    {
        Rect rect = border;
#if UNITY_EDITOR
        rect = worldBorder;
#endif
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }
}