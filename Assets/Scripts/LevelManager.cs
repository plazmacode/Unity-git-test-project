using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private Vector2Int startPosition;

    [SerializeField]
    private Vector2Int goalPosition;

    private List<Vector2Int> waypoints;

    private Stack<Node> path;

    public int WaypointCount { get; set; }

    public Stack<Node> Path
    {
        get
        {
            if (path == null)
            {
                CalculateWaypoints();
            }
            return new Stack<Node>(new Stack<Node>(path));
        }
    }

    public Vector2Int StartPosition { get => startPosition; set => startPosition = value; }
    public List<Vector2Int> Waypoints { get => waypoints; set => waypoints = value; }

    public void RecalculatePath()
    {
        // Calculate Sequential
        if (TestController.Instance.SequentialMode)
        {
            path = AStar.Instance.GetWaypointsPath(waypoints);
        }
        // Calculate shortest
        else
        {
            path = AStar.Instance.GetShortestWaypointPath(waypoints);
        }
        TileManager.Instance.ColorPath(path, Waypoints);
        InterfaceManager.Instance.UpdateLineRendererPath(path);
    }

    private void Start()
    {
        CalculateWaypoints();
    }

    public void CalculateWaypoints()
    {
        List<Vector2Int> newWaypoints = new List<Vector2Int>();

        newWaypoints.Add(startPosition);

        if (TestController.Instance.MinWaypointMode)
        {
            newWaypoints = AStar.Instance.GetMinWaypointMode(newWaypoints[0], WaypointCount, TestController.Instance.MinWaypointDistance);
        } else
        {
            for (int i = 0; i < WaypointCount; i++)
            {
                newWaypoints.Add(AStar.Instance.GetRandomNodePosition());
            }
        }

        newWaypoints.Add(goalPosition);

        // Calculate Sequential
        if (TestController.Instance.SequentialMode)
        {
            path = AStar.Instance.GetWaypointsPath(newWaypoints);
        }
        // Calculate shortest
        else
        {
            path = AStar.Instance.GetShortestWaypointPath(newWaypoints);
        }

        Waypoints = newWaypoints;
        InterfaceManager.Instance.UpdateLineRendererPath(path);
        TileManager.Instance.ColorPath(path, Waypoints);
    }


    public void GeneratePath(bool showDebug)
    {
        path = AStar.Instance.GetPath(StartPosition, goalPosition, showDebug);
    }

    public void Update()
    {
        // Testing code
        // Used for showing / generating A* with waypoints
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Generates path and colors it because showDebug is true
            //GeneratePath(true);

            // Generate Waypoints path
            if (path == null)
            {
                CalculateWaypoints();
            }
            TileManager.Instance.ColorPath(path, Waypoints);
        }
    }
}
