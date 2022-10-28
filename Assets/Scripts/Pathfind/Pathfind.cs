using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate bool IsExcluded(Node node);

public class Pathfind : MonoBehaviour
{
    private WalkableGrid grid;
    private List<Node> gizmosPath;

    private void Start()
    {
        grid = FindObjectOfType<WalkableGrid>();
    }

    public Vector3 Direction(Vector3 start, Vector3 end, IsExcluded isExcluded = null)
    {
        Node startNode = grid.NodeFromWorldPoint(start);
        Node endNode = grid.ClosestWalkableNode(end);
        if (isExcluded(startNode) || isExcluded(endNode))
        {
            return Vector3.zero;
        }
        List<Node> path = ThetaStar(startNode, endNode, isExcluded);
        if (path == null)
        {
            path = new List<Node>{ startNode, endNode };
        }
        gizmosPath = path;
        return Vector3.Normalize(path[1].position - startNode.position);
    }

    void OnDrawGizmos()
    {
        if (grid != null && gizmosPath != null)
        {
            int i = 0;
            foreach (Node n in gizmosPath)
            {
                Gizmos.color = i == 1 ? Color.green : Color.blue;
                Gizmos.DrawCube(MovableObject.GroundScreenCoordinates(n.position) + 0.1f * Vector3.forward, MovableObject.GroundScreenCoordinates(Vector3.one * (grid.nodeRadius * 2f)));
                i++;
            }
        }
    }

    private float Heuristic(Node a, Node b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }

    private List<Node> BuildPath(Dictionary<Node, Node> cameFrom, Node start, Node end)
    {
        Node current = end;
        List<Node> path = new();
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        if (start == end)
        {
            path.Add(end);
        }
        path.Add(start);
        path.Reverse();
        return path;
    }

    private List<Node> ThetaStar(Node start, Node end, IsExcluded isExcluded = null)
    {
        PriorityQueue<Node> frontier = new();
        frontier.Enqueue(start, 0);
        Dictionary<Node, Node> cameFrom = new(){[start] = null};
        Dictionary<Node, float> costSoFar = new(){[start] = 0};
        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();
            foreach (Node node in grid.Neighbors(current))
            {
                if (isExcluded != null && isExcluded(node))
                {
                    continue;
                }
                float newCost;
                if (Mathf.Abs(node.x - current.x) == 1 || Mathf.Abs(node.y - current.y) == 1)
                {
                    newCost = costSoFar[current] + 1;
                }
                else
                {
                    newCost = costSoFar[current] + 1.41f;
                }
                if (!costSoFar.ContainsKey(node) || newCost < costSoFar[node])
                {
                    costSoFar[node] = newCost;
                    float priority = newCost + Heuristic(end, node);
                    frontier.Enqueue(node, priority);
                    if (current != start && grid.LineOfSight(cameFrom[current], node))
                    {
                        cameFrom[node] = cameFrom[current];
                    }
                    else
                    {
                        cameFrom[node] = current;
                    }
                }
            }
            if (current == end)
            {
                return BuildPath(cameFrom, start, end);
            }
        }
        return null;
    }
}
