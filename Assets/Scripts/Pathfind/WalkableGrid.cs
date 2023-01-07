using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MovableObject))]
public class WalkableGrid : BaseComponent
{
    public Vector3 gridWorldSize;
    public float nodeRadius;

    private Node[,] grid;
    [HideInInspector] public MovableObject movableObject;

    private float NodeDiameter => nodeRadius * 2;
    private int GridSizeX => Mathf.RoundToInt(gridWorldSize.x / NodeDiameter);
    private int GridSizeZ => Mathf.RoundToInt(gridWorldSize.z / NodeDiameter);

    private void Awake()
    {
        movableObject = GetComponent<MovableObject>();
    }

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[GridSizeX, GridSizeZ];
        for (var x = 0; x < GridSizeX; x++)
        {
            for (var z = 0; z < GridSizeZ; z++)
            {
                var worldPoint = movableObject.WorldPosition + Vector3.right * (x * NodeDiameter + nodeRadius) +
                                 Vector3.forward * (z * NodeDiameter + nodeRadius);
                grid[x, z] = new Node(x, z, true, worldPoint);
            }
        }

        var barriers = CachedObjectsManager.Instance.GetObjects<Hitbox>("Barrier").ToArray();

        foreach (var barrier in barriers)
        {
            var bottomLeftPoint = barrier.WorldPosition - barrier.size / 2;
            var topRightPoint = barrier.WorldPosition + barrier.size / 2;
            if (!IsInside(bottomLeftPoint) && !IsInside(topRightPoint)) continue;
            var bottomLeftIndex = IndexFromWorldPoint(bottomLeftPoint) - Vector2Int.one;
            var topRightIndex = IndexFromWorldPoint(topRightPoint) + Vector2Int.one;
            for (var x = bottomLeftIndex.x; x <= topRightIndex.x; x++)
            {
                for (var y = bottomLeftIndex.y; y <= topRightIndex.y; y++)
                {
                    if (x >= 0 && x < GridSizeX && y >= 0 && y < GridSizeZ)
                    {
                        grid[x, y].walkable = false;
                    }
                }
            }
        }
    }

    private Vector2Int IndexFromWorldPoint(Vector3 worldPosition)
    {
        var percentX = Mathf.Clamp01((worldPosition.x - movableObject.WorldPosition.x) / gridWorldSize.x);
        var percentZ = Mathf.Clamp01((worldPosition.z - movableObject.WorldPosition.z) / gridWorldSize.z);
        var x = Mathf.RoundToInt((GridSizeX - 1) * percentX);
        var y = Mathf.RoundToInt((GridSizeZ - 1) * percentZ);
        return new Vector2Int(x, y);
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        var index = IndexFromWorldPoint(worldPosition);
        return grid[index.x, index.y];
    }

    public static bool IsWalkable(Node node, Node[] excluded = null)
    {
        return node.walkable && !(excluded != null && excluded.Contains(node));
    }

    public Node ClosestWalkableNode(Vector3 worldPosition, Node[] excluded = null)
    {
        Queue<Node> frontier = new();
        frontier.Enqueue(NodeFromWorldPoint(worldPosition));
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            if (IsWalkable(current, excluded))
            {
                return current;
            }

            foreach (var node in Neighbors(current))
            {
                frontier.Enqueue(node);
            }
        }

        return null;
    }

    public bool IsInside(Vector3 point)
    {
        var bottomLeft = movableObject.WorldPosition;
        var topRight = bottomLeft + gridWorldSize;
        return !(point.x < bottomLeft.x || point.x > topRight.x || point.z < bottomLeft.z || point.z > topRight.z);
    }

    private List<Node> Neighbors(Node node)
    {
        var x = node.x;
        var y = node.y;
        Vector2Int[] possibleIndices =
        {
            new(x - 1, y - 1), new(x, y - 1), new(x + 1, y - 1), new(x - 1, y), new(x + 1, y), new(x - 1, y + 1),
            new(x, y + 1), new(x + 1, y + 1)
        };
        var sizeX = grid.GetLength(0);
        var sizeY = grid.GetLength(1);
        return (from index in possibleIndices
            where index.x >= 0 && index.x < sizeX && index.y >= 0 && index.y < sizeY
            select grid[index.x, index.y]).ToList();
    }

    public List<Node> WalkableNeighbors(Node node, Node[] excluded = null)
    {
        return Neighbors(node).Where(currentNode => IsWalkable(currentNode, excluded)).ToList();
    }

    public IEnumerable<Node> GetCircle(Vector3 center, float radius)
    {
        List<Node> nodes = new();
        var backLeft = IndexFromWorldPoint(center + radius * Vector3.left + radius * Vector3.back);
        var rightForward = IndexFromWorldPoint(center + radius * Vector3.right + radius * Vector3.forward);
        for (var x = backLeft.x; x < rightForward.x; x++)
        {
            for (var y = backLeft.y; y < rightForward.y; y++)
            {
                if (Vector3.Distance(center, grid[x, y].position) < radius)
                {
                    nodes.Add(grid[x, y]);
                }
            }
        }

        return nodes.ToArray();
    }

    public bool LineOfSight(Node a, Node b, Node[] excluded = null)
    {
        var x0 = a.x;
        var y0 = a.y;
        var x1 = b.x;
        var y1 = b.y;

        var dx = Mathf.Abs(x1 - x0);
        var sx = x0 < x1 ? 1 : -1;
        var dy = -Mathf.Abs(y1 - y0);
        var sy = y0 < y1 ? 1 : -1;
        var err = dx + dy;

        while (true)
        {
            if (!IsWalkable(grid[x0, y0], excluded))
            {
                return false;
            }

            if (x0 == x1 && y0 == y1) return true;
            int e2 = 2 * err;
            if (e2 >= dy)
            {
                err += dy;
                x0 += sx;
            }

            if (e2 <= dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!movableObject) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(MovableObject.GroundScreenCoordinates(movableObject.WorldPosition + gridWorldSize / 2),
            MovableObject.GroundScreenCoordinates(gridWorldSize));
    }

    private void OnDrawGizmosSelected()
    {
        if (grid == null) return;
        foreach (var n in grid)
        {
            Gizmos.color = n.walkable ? Color.white : Color.red;
            Gizmos.DrawCube(MovableObject.GroundScreenCoordinates(n.position),
                MovableObject.GroundScreenCoordinates(Vector3.one * (NodeDiameter * 0.75f)));
        }
    }
}

public class Node
{
    public readonly int x;
    public readonly int y;
    public bool walkable;
    public Vector3 position;

    public Node(int x, int y, bool walkable, Vector3 position)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
        this.position = position;
    }
}