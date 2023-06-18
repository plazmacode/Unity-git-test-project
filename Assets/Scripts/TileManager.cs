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

    private bool showWalkables = true;

    private Vector2Int firstTile = new Vector2Int(-12, 5);
    private Vector2Int lastTile = new Vector2Int(8, -7);

    private Dictionary<Vector2Int, Tile> LevelTiles = new Dictionary<Vector2Int, Tile>();

    private Tilemap tilemap;

    [SerializeField]
    private List<Sprite> walkableSprites = new List<Sprite>();

    public void Start()
    {
        tilemap = GetComponent<Tilemap>();
        SetupTiles();
    }

    /// <summary>
    /// Updates walkable bools on tiles
    /// </summary>
    public void SetupTiles()
    {
        for (int x = firstTile.x; x <= lastTile.x; x++)
        {
            for (int y = firstTile.y; y >= lastTile.y; y--)
            {
                Vector2Int levelTilesPosition = new Vector2Int(x, y);
                Vector3Int tileMapPosition = new Vector3Int(x, y, 0);
                Tile newTile = new Tile(levelTilesPosition);

                // Check if current Tiles sprite is a walkableSprite
                if (walkableSprites.Contains(tilemap.GetSprite(tileMapPosition)))
                {
                    newTile.Walkable = true;
                    newTile.IsPath = true;

                    // Green color overlay for walkable tiles
                    if (showWalkables)
                    {
                        tilemap.SetTileFlags(tileMapPosition, TileFlags.None);
                        tilemap.SetColor(tileMapPosition, new Color32(87, 255, 151, 255));
                        tilemap.SetTileFlags(tileMapPosition, TileFlags.LockColor);
                    }
                }
                LevelTiles.Add(levelTilesPosition, newTile);
            }
        }
    }

    public Tile GetTile(Vector2Int position)
    {
        if (LevelTiles.TryGetValue(position, out Tile tile))
        {
            return tile;
        }

        return null; // Or throw an exception if desired
    }

    public void SetTile(Vector2Int position, Tile tile)
    {
        if (LevelTiles.ContainsKey(position))
        {
            LevelTiles[position] = tile;
        }
    }
}

public class Tile
{
    public bool Walkable = false;
    public bool IsPath = false;

    public Vector2Int Position;
    public Tile(Vector2Int position)
    {
        Position = position;
    }

    public Tile (int x, int y, int z)
    {
        Position = new Vector2Int(x, y);
    }
    
    // Possible variables:
    // Tower placed on tile
    // Enemy / enemy amount on tile

    // Tile bonus
    // Tile terrain/biome type
}