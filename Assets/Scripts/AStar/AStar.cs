using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : MonoBehaviour
{
    private Node current;

    private HashSet<Node> openList;
    private HashSet<Node> closedList;

    private Stack<Node> path;

    private Vector2Int startPosition;

    private Vector2Int goalPosition;

    private HashSet<Vector2Int> blockedNodes = new HashSet<Vector2Int>();

    [SerializeField]
    private bool pathOnly = true;

    /// <summary>
    /// DANGEROUS IMPLEMENTATION: Changing AllNodes while AStar is running is a bad idea. Add custom function later to update AllNodes
    /// </summary>
    public Dictionary<Vector2Int, Node> AllNodes { get; set; } = new Dictionary<Vector2Int, Node>();

    private static AStar instance;
    public static AStar Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AStar>();
            }
            return instance;
        }
    }

    private void Initialize()
    {
        current = GetNode(startPosition);

        openList = new HashSet<Node>();
        closedList = new HashSet<Node>();
        openList.Add(current);
    }

    public Stack<Node> GetPath(Vector2Int start, Vector2Int goal, bool colorPath = false)
    {
        startPosition = start;
        goalPosition = goal;

        if (current == null)
        {
            Initialize();
        }
        int i = 0;
        while (openList.Count > 0 && path == null)
        {
            List<Node> neighbors = FindNeighbors(current.Position);

            ExamineNeighbors(neighbors, current);

            UpdateCurrentNode(ref current);

            path = GeneratePath(current);
            i++;
        }

        if (colorPath)
        {
            TileManager.Instance.ColorPathfinding(openList, closedList, AllNodes, startPosition, goalPosition, path);
        }
        return path;
    }

    private List<Node> FindNeighbors(Vector2Int parentPosition)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int neighborPosition = new Vector2Int(parentPosition.x - x, parentPosition.y - y);
                if (y != 0 || x != 0)
                {
                    Node n;
                    AllNodes.TryGetValue(neighborPosition, out n);
                    if (neighborPosition != startPosition && n != null)
                    {
                        Node neighbor = GetNode(neighborPosition);
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    private void ExamineNeighbors(List<Node> neighbors, Node current)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            Node neighbor = neighbors[i];

            if (!ConnectedDiagonally(current, neighbor))
            {
                // skip neighbor if not connected diagonally
                continue;
            }

            if (!current.IsPath)
            {
                // Skip current if it is not a path
                if (pathOnly)
                {
                    continue;
                }
            }

            int gScore = DetermineGScore(neighbors[i].Position, current.Position);

            if (openList.Contains(neighbor))
            {
                if (current.G + gScore < neighbor.G)
                {
                    CalculateValues(current, neighbor, gScore);
                }
            }
            else if (!closedList.Contains(neighbor))
            {
                CalculateValues(current, neighbor, gScore);

                openList.Add(neighbor);
            }
        }
    }

    private void GetBlockedNodes()
    {
        foreach (Node node in AllNodes.Values)
        {
            // Add nodes that are not walkable
            // For static path also add nodes that are not a path
            if (!node.Walkable)
            {
                blockedNodes.Add(node.Position);
            }
            if (!node.IsPath && pathOnly)
            {
                blockedNodes.Add(node.Position);
            }
        }
    }

    private bool ConnectedDiagonally(Node currentNode, Node neighbor)
    {
        Vector2Int direct = currentNode.Position - neighbor.Position;

        Vector2Int first = new Vector2Int(current.Position.x + (direct.x * -1), current.Position.y);
        Vector2Int second = new Vector2Int(current.Position.x, current.Position.y + (direct.y * -1));

        GetBlockedNodes();

        //Check blocked nodes list
        if (blockedNodes.Contains(first) || blockedNodes.Contains(second))
        {
            return false;
        }

        // Check list of nodes only
        // This way only works if using ONLY nodes with IsPath set true
        // ie. Static set path that doesn't change when adding towers.
        //if (AllNodes[first] != null || AllNodes[(second)] != null)
        //{
        //    return false;
        //}

        return true;
    }

    private void CalculateValues(Node parent, Node neighbor, int cost)
    {
        neighbor.Parent = parent;

        neighbor.G = parent.G + cost;

        neighbor.H = ((Math.Abs((neighbor.Position.x - goalPosition.x)) + Math.Abs((neighbor.Position.y - goalPosition.y))) * 10);

        neighbor.F = neighbor.G + neighbor.H;
    }

    private int DetermineGScore(Vector2Int neighbor, Vector2Int current)
    {
        int gScore = 0;
        int x = current.x - neighbor.x;
        int y = current.y - neighbor.y;
        
        if (Math.Abs(x - y) % 2 == 1) {
            gScore = 10;
        }
        else
        {
            gScore = 14;
        }
        return gScore;
    }

private void UpdateCurrentNode(ref Node current)
    {
        openList.Remove(current);

        closedList.Add(current);

        if (openList.Count > 0)
        {
            // Find node with lowest F score
            current = openList.OrderBy(x => x.F).First();
        }
    }

    private Node GetNode(Vector2Int position)
    {
        if (AllNodes.ContainsKey(position))
        {
            return AllNodes[position];
        }
        return null;
    }

    private Stack<Node> GeneratePath(Node current)
    {
        if (current.Position == goalPosition)
        {
            Stack<Node> finalPath = new Stack<Node>();

            while (current.Position != startPosition)
            {
                finalPath.Push(current);

                current = current.Parent;
            }

            return finalPath;
        }

        return null;
    }
}
