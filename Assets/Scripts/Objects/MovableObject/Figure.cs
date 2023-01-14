using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class Figure : BaseComponent
{
    public MovableObject movableObject;

    private new Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    protected override void Update()
    {
        base.Update();
        if (!Application.isPlaying && !movableObject) return;
        transform.localPosition = GameEntity.ScreenCoordinates(movableObject.WorldPosition.y * Vector3.up);
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        renderer.sortingOrder = movableObject.SortingOrder;
    }
}