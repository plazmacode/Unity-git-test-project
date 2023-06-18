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
            string walkableStatus = tile.Walkable ? "is Walkable" : "is not Walkable";
            cellPosition = tile.Position;
            Debug.Log($"Clicked cell position: {cellPosition} {walkableStatus}");
        }
    }
}
