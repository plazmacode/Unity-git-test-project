using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public TileValue Tile { get; set; }
    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }
    public Node Parent { get; set; }
    public Vector2Int Position { get; set; }

    /// <summary>
    /// Currently true because nodes are only created when a tile is walkable
    /// </summary>
    public bool Walkable = true;

    public bool IsPath = false;

    public Node (TileValue tileRef)
    {
        Tile = tileRef;
        Position = Tile.Position;
        AStar.Instance.AllNodes.Add(Position, this);
    }

}
