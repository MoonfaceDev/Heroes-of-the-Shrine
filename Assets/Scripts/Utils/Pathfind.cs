using System.Collections.Generic;
using UnityEngine;

public class Pathfind : MonoBehaviour
{
    private WalkableGrid grid;

    private void Start()
    {
        grid = FindObjectOfType<WalkableGrid>();
    }

    public Vector3 Direction(Vector3 start, Vector3 end)
    {
        Node startNode = grid.ClosestWalkableNode(start);
        Node endNode = grid.ClosestWalkableNode(end);
        List<Node> path = ThetaStar(startNode, endNode);
        return Vector3.Normalize(path[1].position - startNode.position);
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

    private List<Node> ThetaStar(Node start, Node end)
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
