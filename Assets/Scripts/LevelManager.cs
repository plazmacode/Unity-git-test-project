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

    public Stack<Node> Path
    {
        get
        {
            if (path == null)
            {
                CalculateWaypoints(1);
            }
            return new Stack<Node>(new Stack<Node>(path));
        }
    }

    public Vector2Int StartPosition { get => startPosition; set => startPosition = value; }
    public List<Vector2Int> Waypoints { get => waypoints; set => waypoints = value; }

    public void CalculateWaypoints(int waypointAmount)
    {
        path = AStar.Instance.GetWaypointsPath(waypointAmount, out waypoints, startPosition, goalPosition);

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
                CalculateWaypoints(1);
            }
            TileManager.Instance.ColorPath(path, Waypoints);
        }
    }
}
