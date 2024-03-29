﻿using UnityEngine;

/// <summary>
/// Component that updates a character's sprite according to its <see cref="GameEntity"/>
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class Figure : EntityBehaviour
{
    /// <value>
    /// Character's <see cref="GameEntity"/>
    /// </value>
    public GameEntity movableEntity;

    private new Renderer renderer;

    protected override void Awake()
    {
        base.Awake();
        renderer = GetComponent<Renderer>();
    }

    protected override void Update()
    {
        base.Update();
        if (!Application.isPlaying && !movableEntity) return;
        transform.localPosition = new Vector3(transform.localPosition.x, GameEntity.ScreenCoordinates(movableEntity.WorldPosition.y * Vector3.up).y, transform.localPosition.z);
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        renderer.sortingOrder = movableEntity.SortingOrder;
    }
}