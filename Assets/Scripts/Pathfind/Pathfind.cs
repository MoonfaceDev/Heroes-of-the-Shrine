using System.Collections.Generic;
using UnityEngine;

public class Pathfind : MonoBehaviour
{
    private WalkableGrid grid;
    private List<Node> gizmosPath;

    private void Start()
    {
        grid = FindObjectOfType<WalkableGrid>();
    }

    public Vector3 Direction(Vector3 start, Vector3 end, Node[] excluded = null)
    {
        var startNode = grid.NodeFromWorldPoint(start);
        var endNode = grid.ClosestWalkableNode(end, excluded);
        var path = ThetaStar(startNode, endNode, excluded) ?? new List<Node>{ startNode, endNode };
        gizmosPath = path;
        return Vector3.Normalize(path.Count == 2 ? end - start : path[1].position - startNode.position);
    }

    private void OnDrawGizmos()
    {
        if (grid == null || gizmosPath == null) return;
        var i = 0;
        foreach (var n in gizmosPath)
        {
            Gizmos.color = i == 1 ? Color.green : Color.blue;
            Gizmos.DrawCube(MovableObject.GroundScreenCoordinates(n.position) + 0.1f * Vector3.forward, MovableObject.GroundScreenCoordinates(Vector3.one * (grid.nodeRadius * 2f)));
            i++;
        }
    }

    private static float Heuristic(Node a, Node b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }

    private static List<Node> BuildPath(IReadOnlyDictionary<Node, Node> cameFrom, Node start, Node end)
    {
        var current = end;
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

    private List<Node> ThetaStar(Node start, Node end, Node[] excluded = null)
    {
        PriorityQueue<Node> frontier = new();
        frontier.Enqueue(start, 0);
        Dictionary<Node, Node> cameFrom = new(){[start] = null};
        Dictionary<Node, float> costSoFar = new(){[start] = 0};
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            foreach (var node in grid.WalkableNeighbors(current, excluded))
            {
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
                    var priority = newCost + Heuristic(end, node);
                    frontier.Enqueue(node, priority);
                    if (current != start && grid.LineOfSight(cameFrom[current], node, excluded))
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
