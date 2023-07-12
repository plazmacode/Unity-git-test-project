﻿using System;
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

    public Stack<Node> GetWaypointsPath(List<Vector2Int> waypoints)
    {
        List<Node> waypointPath = new List<Node>();

        for (int i = 0; i < waypoints.Count-1; i++)
        {
            // Path can become null sometimes?
            Stack<Node> pathToWaypoint = GetPath(waypoints[i], waypoints[i+1]);

            waypointPath.AddRange(pathToWaypoint);
        }
        //Stack<Node> pathToGoal = GetPath(waypoints[waypoints.Count-1], goal); // If path is null maybe try a new waypoint?

        //// Add stack 2 to stack 1
        //int counter2 = 0; // What if a while loop stucks our program 🤔
        //// We sometimes get a null reference here because the path from GetPath() can be null
        //while (waypointPath.Count > 0 && counter2 < 200)
        //{
        //    counter2++;
        //    pathToGoal.Push(waypointPath.Pop());
        //}

        waypointPath.Reverse();
        Stack<Node> pathStack = new Stack<Node>();

        for (int i = 0; i < waypointPath.Count; i++)
        {
            pathStack.Push(waypointPath[i]);
        }

        path = pathStack;
        
        return path;
    }

    public Stack<Node> GetShortestWaypointPath(List<Vector2Int> waypoints)
    {
        List<Vector2Int> waypointCopy = new List<Vector2Int>(waypoints);
        List<Vector2Int> shortestWaypointOrder = new List<Vector2Int>();
        shortestWaypointOrder.Add(waypoints[0]);

        while (waypointCopy.Count > 2)
        {
            Stack<Node> shortestPathToWaypoint = GetPath(shortestWaypointOrder[shortestWaypointOrder.Count -1], waypointCopy[1]);
            Vector2Int nextWaypoint = waypointCopy[1];
            for (int i = 1; i < waypointCopy.Count - 1; i++)
            {
                Stack<Node> wayPointPath = GetPath(shortestWaypointOrder[shortestWaypointOrder.Count -1], waypointCopy[i]);
                if (wayPointPath.Count < shortestPathToWaypoint.Count)
                {
                    shortestPathToWaypoint = wayPointPath;
                    nextWaypoint = waypointCopy[i];
                }
            }

            shortestWaypointOrder.Add(nextWaypoint);
            waypointCopy.Remove(nextWaypoint);
        }
        shortestWaypointOrder.Add(waypoints[waypoints.Count -1]);


        return GetWaypointsPath(shortestWaypointOrder);
    }

    public List<Vector2Int> GetMinWaypointMode(Vector2Int start, int waypointCount, int minWaypointDistance)
    {
        List<Vector2Int> waypoints = new List<Vector2Int>();
        waypoints.Add(start);

        while (waypoints.Count -1 < waypointCount)
        {
            Vector2Int randomPosition = GetRandomNodePosition();

            bool accepted = true;
            for (int i = 0; i < waypoints.Count; i++)
            {
                Stack<Node> currentPath = GetPath(waypoints[i], randomPosition);
                if (currentPath.Count < minWaypointDistance)
                {
                    accepted = false;
                }
            }

            if (accepted)
            {
                waypoints.Add(randomPosition);
            }

        }

        return waypoints;
    }

    public Vector2Int GetRandomNodePosition()
    {
        // UnityEngine.Random uses same seed as terrain. See InitState()
        // Terrain currently has a set seed in the editor.
        // Random waypoints are always the same with this implementation.
        Vector2Int position = AllNodes
            .ElementAt(UnityEngine.Random.Range(0, AllNodes.Count))
            .Value.Position;

        return position;
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

        if (path == null)
        {
            Debug.Log($"Path size is null? start: {start}, goal: {goal}");
        }

        return path;
    }

    public List<Node> FindNeighbors(Vector2Int parentPosition)
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

                        neighbor.Walkable = !neighbor.Tile.HasTower;

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

    public Node GetNode(Vector2Int position)
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
