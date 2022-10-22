using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public string targetTag;
    public Rect worldBorder;
    public float lerpSpeed = 3.0f;

    private new Camera camera;
    private WalkableGrid walkableGrid;
    private MovableObject target;
    private Vector3 offset;
    private Vector3 targetPosition;
    private Rect border;

    private static Vector2 epsilon = Vector2.one * 0.01f;

    public float CameraHeight => 2f * camera.orthographicSize;

    public float CameraWidth => CameraHeight * camera.aspect;

    public Vector2 CameraSize => new(CameraWidth, CameraHeight);

    private void Awake()
    {
        camera = GetComponent<Camera>();
        walkableGrid = FindObjectOfType<WalkableGrid>();
        target = GameObject.FindGameObjectWithTag(targetTag).GetComponent<MovableObject>();
        border = worldBorder;
    }

    private void Start()
    {
        if (target)
        {
            offset = transform.position - MovableObject.GroundScreenCoordinates(target.position);
        }
    }

    public void Lock(Rect newBorder)
    {
        border = newBorder;
        walkableGrid.GetComponent<MovableObject>().position.x = newBorder.xMin;
        walkableGrid.gridWorldSize.x = newBorder.width;
        walkableGrid.CreateGrid();
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
        Vector3 nextPosition = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
        if (border.Contains((Vector2)nextPosition - CameraSize / 2 + epsilon) && border.Contains((Vector2)nextPosition + CameraSize / 2 - epsilon))
        {
            transform.position = nextPosition;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;
        targetPosition = MovableObject.GroundScreenCoordinates(target.position) + offset;
        MoveCamera(transform.position + (targetPosition.x - transform.position.x) * Vector3.right);
        MoveCamera(transform.position + (targetPosition.y - transform.position.y) * Vector3.up);
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