using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MovableObject))]
public class WalkableGrid : MonoBehaviour
{
	public Vector3 gridWorldSize;
	public float nodeRadius;
	[HideInInspector]
	public Node[,] grid;

	private MovableObject movableObject;
	private float NodeDiameter => nodeRadius * 2;
	private int GridSizeX => Mathf.RoundToInt(gridWorldSize.x / NodeDiameter);
	private int GridSizeZ => Mathf.RoundToInt(gridWorldSize.z / NodeDiameter);

	void Awake()
	{
		movableObject = GetComponent<MovableObject>();
	}

	public void CreateGrid()
	{
		grid = new Node[GridSizeX, GridSizeZ];
		for (int x = 0; x < GridSizeX; x++)
		{
			for (int z = 0; z < GridSizeZ; z++)
			{
				Vector3 worldPoint = movableObject.position + Vector3.right * (x * NodeDiameter + nodeRadius) + Vector3.forward * (z * NodeDiameter + nodeRadius);
				grid[x, z] = new Node(x, z, true, worldPoint);
			}
		}
		Hitbox[] hitboxes = FindObjectsOfType<Hitbox>();    

		foreach (Hitbox hitbox in hitboxes)
		{
			if (hitbox.CompareTag("Barrier") && (IsInside(hitbox.position) || IsInside(hitbox.position + hitbox.size)))
			{
				Vector2Int bottomLeftIndex = IndexFromWorldPoint(hitbox.position) - new Vector2Int(1, 1);
				Vector2Int topRightIndex = IndexFromWorldPoint(hitbox.position + hitbox.size) + new Vector2Int(1, 1);
				for (int x = bottomLeftIndex.x; x <= topRightIndex.x; x++)
				{
					for (int y = bottomLeftIndex.y; y <= topRightIndex.y; y++)
					{
						if (x >= 0 && x < GridSizeX && y >= 0 && y < GridSizeZ)
						{
							grid[x, y].walkable = false;
						}
					}
				}
			}
		}
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		Vector2Int index = IndexFromWorldPoint(worldPosition);
		return grid[index.x, index.y];
	}

	public Node ClosestWalkableNode(Vector3 worldPosition)
	{
		Queue<Node> frontier = new();
		Node current;
		frontier.Enqueue(NodeFromWorldPoint(worldPosition));
		while (frontier.Count > 0)
		{
			current = frontier.Dequeue();
			if (current.walkable)
			{
				return current;
			}
			foreach (Node node in RawNeighbors(current))
			{
				frontier.Enqueue(node);
			}
		}
		return null;
	}

	public bool IsInside(Vector3 point)
    {
		Vector3 bottomLeft = movableObject.position;
		Vector3 topRight = bottomLeft + gridWorldSize;
		return !(point.x < bottomLeft.x || point.x > topRight.x || point.z < bottomLeft.z || point.z > topRight.z);
	}

	public Vector2Int IndexFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = Mathf.Clamp01((worldPosition.x - movableObject.position.x) / gridWorldSize.x);
		float percentZ = Mathf.Clamp01((worldPosition.z - movableObject.position.z) / gridWorldSize.z);
		int x = Mathf.RoundToInt((GridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((GridSizeZ-1) * percentZ);
		return new Vector2Int(x, y);
	}

	public List<Node> Neighbors(Node node)
	{
		int x = node.x;
		int y = node.y;
		Vector2Int[] possibleIndices = new Vector2Int[] { new Vector2Int(x - 1, y - 1), new Vector2Int(x, y - 1), new Vector2Int(x + 1, y - 1), new Vector2Int(x - 1, y), new Vector2Int(x + 1, y), new Vector2Int(x - 1, y + 1), new Vector2Int(x, y + 1), new Vector2Int(x + 1, y + 1) };
		List<Node> neighbors = new();
		int sizeX = grid.GetLength(0);
		int sizeY = grid.GetLength(1);
		foreach (Vector2Int index in possibleIndices)
		{
			if (index.x >= 0 && index.x < sizeX && index.y >= 0 && index.y < sizeY)
			{
				Node neighbor = grid[index.x, index.y];
				if (neighbor.walkable)
				{
					neighbors.Add(neighbor);
				}
			}
		}
		return neighbors;
	}

	public List<Node> RawNeighbors(Node node)
	{
		int x = node.x;
		int y = node.y;
		Vector2Int[] possibleIndices = new Vector2Int[] { new Vector2Int(x - 1, y - 1), new Vector2Int(x, y - 1), new Vector2Int(x + 1, y - 1), new Vector2Int(x - 1, y), new Vector2Int(x + 1, y), new Vector2Int(x - 1, y + 1), new Vector2Int(x, y + 1), new Vector2Int(x + 1, y + 1) };
		List<Node> neighbors = new();
		int sizeX = grid.GetLength(0);
		int sizeY = grid.GetLength(1);
		foreach (Vector2Int index in possibleIndices)
		{
			if (index.x >= 0 && index.x < sizeX && index.y >= 0 && index.y < sizeY)
			{
				neighbors.Add(grid[index.x, index.y]);
			}
		}
		return neighbors;
	}

	public bool LineOfSight(Node a, Node b)
	{
		int x0 = a.x;
		int y0 = a.y;
		int x1 = b.x;
		int y1 = b.y;

		int dx = Mathf.Abs(x1 - x0);
		int sx = x0 < x1 ? 1 : -1;
		int dy = -Mathf.Abs(y1 - y0);
		int sy = y0 < y1 ? 1 : -1;
		int err = dx + dy;

		while (true)
		{
			if (!grid[x0, y0].walkable)
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

	void OnDrawGizmos()
	{
		MovableObject movableObject = GetComponent<MovableObject>();
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(MovableObject.GroundScreenCoordinates(movableObject.position + gridWorldSize / 2), MovableObject.GroundScreenCoordinates(gridWorldSize));


		if (grid != null)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube(MovableObject.GroundScreenCoordinates(n.position), MovableObject.GroundScreenCoordinates(Vector3.one * (NodeDiameter * 0.75f)));
			}
		}
	}
}

public class Node
{
	public int x;
	public int y;
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