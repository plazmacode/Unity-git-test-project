using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LogCellPosition : MonoBehaviour
{
    // Start is called before the first frame update
    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int cellPosition = (Vector2Int)tilemap.WorldToCell(mouseWorldPos);
            Tile tile = TileManager.Instance.GetTile(cellPosition);
            if (tile == null)
            {
                Debug.Log($"Pos:{cellPosition}: No Custom tile data for this tilemap tile. See TileManager for more.");
            } else
            {
                if (tile.TileNode != null)
                {
                    string walkableStatus = tile.TileNode.Walkable ? "is Walkable" : "is not Walkable";
                    string pathStatus = tile.IsPath ? "is Path" : "is not Path";
                    cellPosition = tile.Position;
                    Debug.Log($"Pos:{cellPosition}: {walkableStatus}: {pathStatus}");
                } else
                {
                    Debug.Log($"Pos{cellPosition}: Tile has no node");
                }
            }
        }
    }
}
