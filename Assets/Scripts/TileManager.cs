using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    // Possible variables:
    // Start/End tiles
    // Total tiles
    // First and last cell position used in level

    private static TileManager instance;

    public static TileManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TileManager>();
            }

            return instance;
        }
    }

    // Start and end position of tilemap.
    private Vector2Int firstTile = new Vector2Int(-12, 5);
    private Vector2Int lastTile = new Vector2Int(8, -7);

    private Dictionary<Vector2Int, Tile> levelTiles = new Dictionary<Vector2Int, Tile>();

    /// <summary>
    /// Property for accessing the Tilemap
    /// </summary>
    public Tilemap TileMap { get; set; }

    /// <summary>
    /// List of Sprites considered walkable
    /// </summary>
    [SerializeField]
    private List<Sprite> walkableSprites = new List<Sprite>();

    /// <summary>
    /// Prefab used to display tiles pathfinding information on canvas
    /// </summary>
    [SerializeField]
    private GameObject debugTextPrefab;

    /// <summary>
    /// canvas to add pathfinding UI prefab to.
    /// </summary>
    [SerializeField]
    private Canvas debugCanvas;

    [SerializeField]
    private bool onlyPathWalkable;

    public bool OnlyPathWalkable
    {
        get
        {
            return onlyPathWalkable;
        }
    }

    // bool to determine if walkables should be colored.
    [SerializeField]
    private bool showWalkables = false;

    // Colors of pathfinding
    // NOTICE: the color is applied to the tile so it will be mixed.
    // Change the tiles sprite to change this behaviour.
    private Color32 startColor = new Color32(50, 255, 50, 255);
    private Color32 goalColor = new Color32(255, 50, 50, 255);
    private Color32 openColor = new Color32(50, 50, 255, 255);
    private Color32 closedColor = new Color32(52, 235, 219, 255);
    private Color32 pathColor = new Color32(255, 0, 255, 255);

    // List used in tutorial, idk what it does.
    private List<GameObject> debugObjects = new List<GameObject>();

    public void Start()
    {
        TileMap = GetComponent<Tilemap>();
        SetupTiles();
    }

    /// <summary>
    /// Color tiles according to pathfinding data.
    /// </summary>
    /// <param name="openList"></param>
    /// <param name="closedList"></param>
    /// <param name="allNodes"></param>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <param name="path"></param>
    public void ColorPathfinding(HashSet<Node> openList, HashSet<Node> closedList, Dictionary<Vector2Int, Node> allNodes, Vector2Int start, Vector2Int goal, Stack<Node> path = null)
    {
        foreach (Node node in openList)
        {
            ColorTile(node.Position, openColor);
        }

        foreach (Node node in closedList)
        {
            ColorTile(node.Position, closedColor);
        }

        if (path != null)
        {
            foreach (Node node in path)
            {
                if (node.Position != start && node.Position != goal)
                {
                    ColorTile(node.Position, pathColor);
                }
            }
        }

        ColorTile(start, startColor);
        ColorTile(goal, goalColor);

        // Destroy DebugCanvas children
        for (int i = debugCanvas.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = debugCanvas.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        // Add debugPrefab of Astar neighbor values to canvas
        foreach (KeyValuePair<Vector2Int, Node> node in allNodes)
        {
            if (node.Value.Parent != null)
            {
                GameObject go = Instantiate(debugTextPrefab, debugCanvas.transform);
                go.transform.position = TileMap.CellToWorld((Vector3Int)node.Key);
                debugObjects.Add(go);
                GenerateDebugText(node.Value, go.GetComponent<DebugText>());
            }
        }
    }

    /// <summary>
    /// Update debugPrefab values
    /// </summary>
    /// <param name="node"></param>
    /// <param name="debugText"></param>
    private void GenerateDebugText(Node node, DebugText debugText)
    {
        debugText.P.text = $"P:{node.Position.x},{node.Position.y}";
        debugText.F.text = $"F:{node.F}";
        debugText.G.text = $"G:{node.G}";
        debugText.H.text = $"H:{node.H}";

        debugText.Arrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 180f / Mathf.PI * Mathf.Atan2(node.Parent.Position.y - node.Position.y, node.Parent.Position.x - node.Position.x)));
    }

    public void ColorTile(Vector2Int position, Color color)
    {
        TileMap.SetTileFlags((Vector3Int)position, TileFlags.None);
        TileMap.SetColor((Vector3Int)position, color);
    }

    /// <summary>
    /// Populate levelTiles Dictionary containing Tile class.
    /// Tile creation also creates Nodes used in pathfinding.
    /// </summary>
    public void SetupTiles()
    {
        for (int x = firstTile.x; x <= lastTile.x; x++)
        {
            for (int y = firstTile.y; y >= lastTile.y; y--)
            {
                Vector2Int levelTilesPosition = new Vector2Int(x, y);
                Vector3Int tileMapPosition = new Vector3Int(x, y, 0);

                // Skip node creation if only path tiles should have nodes
                Tile newTile = new Tile(levelTilesPosition, !OnlyPathWalkable);

                // Check if current Tiles sprite is a walkableSprite
                if (walkableSprites.Contains(TileMap.GetSprite(tileMapPosition)))
                {
                    // Add Node to path nodes only
                    // Prevent double creation of nodes with if-statement
                    if (OnlyPathWalkable)
                    {
                        newTile.TileNode = new Node(newTile);
                    }

                    newTile.IsPath = true;

                    // Update tileNode's IsPath variable
                    AStar.Instance.AllNodes[levelTilesPosition].IsPath = true;
                    AStar.Instance.AllNodes[levelTilesPosition].Walkable = true;

                    // Green color overlay for walkable tiles when path used
                    if (showWalkables && OnlyPathWalkable)
                    {
                        ColorTile(levelTilesPosition, new Color32(87, 255, 151, 255));
                        //TileMap.SetTileFlags(tileMapPosition, TileFlags.LockColor);
                    }
                }
                // Green color overlay for walkable tiles when path not used
                if (showWalkables && !OnlyPathWalkable)
                {
                    ColorTile(levelTilesPosition, new Color32(87, 255, 151, 255));
                    //TileMap.SetTileFlags(tileMapPosition, TileFlags.LockColor);
                }
                levelTiles.Add(levelTilesPosition, newTile);
            }
        }
    }

    public Tile GetTile(Vector2Int position)
    {
        if (levelTiles.TryGetValue(position, out Tile tile))
        {
            return tile;
        }

        return null; // Or throw an exception if desired
    }

    public void SetTile(Vector2Int position, Tile tile)
    {
        if (levelTiles.ContainsKey(position))
        {
            levelTiles[position] = tile;
        }
    }
}

/// <summary>
/// Tile class contains variables stored on each Tile.
/// Pathfinding related variables stored on Node class
/// </summary>
public class Tile
{
    public Node TileNode { get; set; }

    public Vector2Int Position;
    public Vector3 WorldPosition;
    public bool IsPath = false;

    /// <summary>
    /// Set isNode false when sprite is a permanent blocking sprite
    /// </summary>
    /// <param name="position"></param>
    /// <param name="isNode"></param>
    public Tile(Vector2Int position, bool hasNode = true)
    {
        Position = position;
        WorldPosition = TileManager.Instance.TileMap.CellToWorld((Vector3Int)position);
        if (hasNode)
        {
            TileNode = new Node(this);
        }
    }
    
    // Possible variables:
    // Tower placed on tile
    // Enemy / enemy amount on tile

    // Tile bonus
    // Tile terrain/biome type
}