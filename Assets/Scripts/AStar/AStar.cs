using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : Singleton<AStar>
{
    private Node current;

    private HashSet<Node> openList;
    private HashSet<Node> closedList;

    private Stack<Node> path;

    private Vector2Int startPosition;

    private Vector2Int goalPosition;

    private HashSet<Vector2Int> blockedNodePositions = new HashSet<Vector2Int>();

    /// <summary>
    /// DANGEROUS IMPLEMENTATION: Changing AllNodes while AStar is running is a bad idea. Add custom function later to Add and Remove nodes. <br></br>
    /// If the AllNodes Dictionary is changed, eg. placing a tower blocking a node the algorithm is using, then the foreach loops will fail
    /// </summary>
    public Dictionary<Vector2Int, Node> AllNodes { get; set; } = new Dictionary<Vector2Int, Node>();

    /// <summary>
    /// Initializes open and clsoed list, sets current ndoe to startPosition.
    /// </summary>
    private void Initialize()
    {
        current = GetNode(startPosition);
        path = null;

        openList = new HashSet<Node>();
        closedList = new HashSet<Node>();
        openList.Add(current);


        foreach (Node node in AllNodes.Values)
        {
            node.Parent = null;
        }
    }

    public Stack<Node> GetPath(Vector2Int start, Vector2Int goal, bool colorPath = false)
    {
        // Clear path variable so AStar can be run again
        // Makes it possible to update path based on nodes that have become blocked

        startPosition = start;
        goalPosition = goal;

        if (current == null)
        {
            Initialize();
        }
        int i = 0;
        while (openList.Count > 0 && path == null && i < 500)
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
        current = null;

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

                        // Only add walkable neighbors to returned list.
                        if (neighbor.Walkable)
                        {
                            neighbors.Add(neighbor);
                        }
                    }
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Find posititions around parent. <br></br>
    /// Used for finding blocked nodes.
    /// </summary>
    /// <param name="parentPosition"></param>
    /// <returns></returns>
    private List<Vector2Int> FindNeighborPositions(Vector2Int parentPosition)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int neighborPosition = new Vector2Int(parentPosition.x - x, parentPosition.y - y);
                if (y != 0 || x != 0)
                {
                    neighbors.Add(neighborPosition);
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

            // Different implementation of Path Only
            // Current implementation only creates nodes walkable nodes on paths, when Path Only is used.
            //if (!current.IsPath)
            //{
            //    // Skip current if it is not a path
            //    if (TileManager.Instance.OnlyPathWalkable)
            //    {
            //        continue;
            //    }
            //}

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

    /// <summary>
    /// Cannot get blocked nodes if there are no node neighbors :(
    /// </summary>
    private void GetBlockedNodes()
    {
        blockedNodePositions = new HashSet<Vector2Int>();
        foreach (Node node in AllNodes.Values)
        {
            // Add nodes that are not walkable
            // For static path also add nodes that are not a path
            if (!node.Walkable)
            {
                // Add THIS position to blocked list
                blockedNodePositions.Add(node.Position);
            }
            
            // Check assumes neighbor nodes exist in Only Path mode
            // They do not in first implementation. See Tile class inside TileManager.
            if (!node.IsPath && TileManager.Instance.OnlyPathWalkable)
            {
                // Add THIS position to blocked list
                blockedNodePositions.Add(node.Position);
            }

            // Assumes nodes are only created if they are walkable.
            // Used because of above check.
            if (TileManager.Instance.OnlyPathWalkable)
            {
                // Get a list of the 8 positions surrounding this node
                List<Vector2Int> neighborPositions = FindNeighborPositions(node.Position);
                
                // Check all 8 positions
                for (int i = 0; i < neighborPositions.Count; i++)
                {
                    Node n;
                    AllNodes.TryGetValue(neighborPositions[i], out n);
                    // If position has no node. Therefore no path
                    if (n == null)
                    {
                        // Add SURROUNDING positions to blocked nodes.
                        blockedNodePositions.Add(neighborPositions[i]);
                    }
                }
            }
        }
    }

    private bool ConnectedDiagonally(Node currentNode, Node neighbor)
    {
        Vector2Int direction = neighbor.Position - currentNode.Position;

        Vector2Int first = new Vector2Int(currentNode.Position.x + direction.x, currentNode.Position.y);
        Vector2Int second = new Vector2Int(currentNode.Position.x, currentNode.Position.y + direction.y);

        GetBlockedNodes();

        //Check blocked nodes list
        if (blockedNodePositions.Contains(first) || blockedNodePositions.Contains(second))
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
