using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class Figure : BaseComponent
{
    public MovableEntity movableEntity;

    private new Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    protected override void Update()
    {
        base.Update();
        if (!Application.isPlaying && !movableEntity) return;
        transform.localPosition = GameEntity.ScreenCoordinates(movableEntity.WorldPosition.y * Vector3.up);
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        renderer.sortingOrder = movableEntity.SortingOrder;
    }
}