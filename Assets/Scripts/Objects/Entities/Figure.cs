using UnityEngine;

/// <summary>
/// Component that updates a character's sprite according to its <see cref="GameEntity"/>
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class Figure : BaseComponent
{
    /// <value>
    /// Character's <see cref="GameEntity"/>
    /// </value>
    public GameEntity movableEntity;

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