using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    private static LevelManager instance;

    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelManager>();
            }

            return instance;
        }
    }

    [SerializeField]
    private Vector2Int startPosition;

    [SerializeField]
    private Vector2Int goalPosition;

    private Stack<Node> path;

    public Stack<Node> Path
    {
        get
        {
            if (path == null)
            {
                GeneratePath(false);
            }
            return new Stack<Node>(new Stack<Node>(path));
        }
    }

    public Vector2Int StartPosition { get => startPosition; set => startPosition = value; }

    public void GeneratePath(bool showDebug)
    {
        path = AStar.Instance.GetPath(StartPosition, goalPosition, showDebug);
    }

    public void Update()
    {
        // Testing code
        // Used for generating A*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Generates path and colors it because showDebug is true
            GeneratePath(true);
        }
    }
}
