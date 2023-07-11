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
    public bool Walkable { get; set; } = true;

    public bool IsPath { get; private set; } = false;

    public Node (TileValue tileRef)
    {
        Tile = tileRef;
        Position = Tile.Position;
        AStar.Instance.AllNodes.Add(Position, this);
    }

    public void SetPath(bool value)
    {
        AStar.Instance.AllNodes[Position].IsPath = value;
    }

    public void SetWalkable(bool value)
    {
        AStar.Instance.AllNodes[Position].Walkable = value;
    }

}
