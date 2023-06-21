using UnityEngine;
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
    private float tileSize = 0.32f;

    /// <summary>
    /// Set isNode false when sprite is a permanent blocking sprite
    /// </summary>
    /// <param name="position"></param>
    /// <param name="isNode"></param>
    public Tile(Vector2Int position, bool hasNode = true)
    {
        Position = position;
        
        // This tiles position using World coordinates
        WorldPosition = TileManager.Instance.TileMap.CellToWorld((Vector3Int)position);

        // Add offset to make position be the middle of the tile.
        // Used to center enemies inside pathfinding
        WorldPosition += new Vector3(tileSize, tileSize, 0);

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