using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Tile Tile { get; set; }
    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }
    public Node Parent { get; set; }
    public Vector2Int Position { get; set; }

    public bool Walkable = true;

    public bool IsPath = false;

    public Node (Tile tileRef)
    {
        Tile = tileRef;
        AStar.Instance.AllNodes.Add(Position, this);
    }

}
