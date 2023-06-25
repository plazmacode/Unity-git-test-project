using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : Singleton<TileManager>
{
    // Possible variables:
    // Start/End tiles
    // Total tiles
    // First and last cell position used in level

    // Start and end position of tilemap.
    [SerializeField]
    private Vector2Int[] tileLimits = new Vector2Int[2] { new Vector2Int(-12, 5), new Vector2Int(8, -7) }; 
    public Vector2Int[] TileLimits { get => tileLimits; set => tileLimits = value; }

    // Array for storing Tiles
    // Tile class stores custom data for each Tile in the Tilemap.
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
        for (int x = TileLimits[0].x; x <= TileLimits[1].x; x++)
        {
            for (int y = TileLimits[0].y; y >= TileLimits[1].y; y--)
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
        CameraMovement c = FindObjectOfType<CameraMovement>();
        c.SetLimits();
    }
    
    /// <summary>
    /// Check if tile is inside limits of play area.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool TileInsideArea(Tile tile)
    {
        if (tile.Position.x < tileLimits[0].x || tile.Position.x > tileLimits[1].x)
            return false;

        // Check if the tile's Y position is within the limits
        if (tile.Position.y < tileLimits[1].y || tile.Position.y > tileLimits[0].y)
            return false;

        // Tile is inside the limits
        return true;
    }

    /// <summary>
    /// Check if tile is inside limits of play area.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool CellPositionInsideArea(Vector2Int position)
    {
        if (position.x < tileLimits[0].x || position.x > tileLimits[1].x)
            return false;

        // Check if the tile's Y position is within the limits
        if (position.y < tileLimits[1].y || position.y > tileLimits[0].y)
            return false;

        // Tile is inside the limits
        return true;
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