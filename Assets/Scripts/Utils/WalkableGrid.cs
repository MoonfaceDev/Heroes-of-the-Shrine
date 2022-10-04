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
	float nodeDiameter;
	[HideInInspector]
	public int gridSizeX, gridSizeZ;

	void Start()
	{
		movableObject = GetComponent<MovableObject>();
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
		CreateGrid();
	}

	void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeZ];
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int z = 0; z < gridSizeZ; z++)
			{
				Vector3 worldPoint = movableObject.position + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
				grid[x, z] = new Node(x, z, true, worldPoint);
			}
		}
		Hitbox[] hitboxes = FindObjectsOfType<Hitbox>();    

		foreach (Hitbox hitbox in hitboxes)
		{
			if (hitbox.CompareTag("Barrier"))
			{
				Vector2Int bottomLeftIndex = IndexFromWorldPoint(hitbox.position);
				Vector2Int topRightIndex = IndexFromWorldPoint(hitbox.position + hitbox.size);
				for (int x = bottomLeftIndex.x; x <= topRightIndex.x; x++)
				{
					for (int y = bottomLeftIndex.y; y <= topRightIndex.y; y++)
					{
						if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeZ)
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
		Queue<Node> frontier = new Queue<Node>();
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

	public Vector2Int IndexFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x - movableObject.position.x) / gridWorldSize.x;
		float percentZ = (worldPosition.z - movableObject.position.z) / gridWorldSize.z;
		int x = Mathf.RoundToInt(gridSizeX * percentX);
		int y = Mathf.RoundToInt(gridSizeZ * percentZ);
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
		Gizmos.DrawWireCube(MovableObject.GroundScreenCoordinates(movableObject.position + gridWorldSize / 2), MovableObject.GroundScreenCoordinates(gridWorldSize));


		if (grid != null)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube(MovableObject.GroundScreenCoordinates(n.position), MovableObject.GroundScreenCoordinates(Vector3.one * (nodeDiameter * 0.75f)));
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