using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileMouseScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Tilemap tilemap;
    private TileValue currentTile;
    private TileValue previousTile;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void Update()
    {
        Vector2Int mousePosition = GetMousePosition();
        TileValue tile;
        // Check if mousePosition is over a tile
        if (TileManager.Instance.CellPositionInsideArea(mousePosition))
        {
            tile = TileManager.Instance.GetTile(mousePosition);
        } else
        {
            currentTile = null;
            return;
        }

        // If tile is null or has no sprite.
        if (tile == null && !tilemap.GetSprite((Vector3Int)tile.Position))
        {
            //Debug.Log($"Pos:{mousePosition}: No Custom tile data for this tilemap tile. See TileManager for more.");
            currentTile = null;
            return;
        }
        else
        {
            // SUCCESS--------- MOUSE OVER TILE--------- 

            currentTile = tile;

            MouseOverTile(tile);
        }

        // --------- MOUSE OVER NEW TILE --------- 
        if (tile != previousTile && tile != null)
        {
            //TileManager.Instance.ColorTile(tile.Position, Color.white);
            previousTile = tile;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //DebugText();
        }
    }

    /// <summary>
    /// Handles buying towers when they are placed on a tile.
    /// </summary>
    /// <param name="tile"></param>
    private void MouseOverTile(TileValue tile)
    {
        // If mouse not over UI and Tower button has been selected.
        if (Input.GetMouseButtonDown(0))
        {
            // The DebugCanvas blocks our tower placement
            TileManager.Instance.ClearDebugCanvas();

            // Checks if mouse over canvas, towers is being placed(a button is clicked) and if tile is not null.
            if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedButton != null && tile != null)
            {
                // Check price and if their already is a tower.
                // Alternatively add an BlacementBlocked bool for more cases of a blocked tile in case of tower placement
                if (GameManager.Instance.Money > GameManager.Instance.ClickedButton.Price && !currentTile.HasTower)
                {
                    // subtract money
                    GameManager.Instance.Money -= GameManager.Instance.ClickedButton.Price;

                    // increase price by base price
                    GameManager.Instance.ClickedButton.Price += GameManager.Instance.ClickedButton.BasePrice;

                    // Place Tower sets ClickedButton to null, meaning variables from their will not be accessible anymore
                    PlaceTower();

                    currentTile.HasTower = true;
                    currentTile.TileNode.SetWalkable(false); // Make enemies not walk over this tile.
                    // Implementation of this can be done differently later.

                    // Implementation 1: Use a HasTower bool with the AStar instead of only a Walkable bool

                    // Implementation 2: Set Walkable automatically when HasTower property is changed.
                }
            }
        }
    }

    /// <summary>
    /// Convert mouse world position to cell position and return. <br></br>
    /// Cell position used by Tilemap class for tiles position
    /// </summary>
    /// <returns></returns>
    private Vector2Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return (Vector2Int)tilemap.WorldToCell(mouseWorldPos);
    }

    private void DebugText()
    {
        if (currentTile.TileNode != null)
        {
            string walkableStatus = currentTile.TileNode.Walkable ? "is Walkable" : "is not Walkable";
            string pathStatus = currentTile.TileNode.IsPath ? "is Path" : "is not Path";
            Debug.Log($"Pos:{currentTile.Position}: {walkableStatus}: {pathStatus}");
        }
        else
        {
            Debug.Log($"Pos{currentTile.Position}: Tile has no node");
        }
    }

    /// <summary>
    /// Instantiates a new tower, sets its layer and runs the BuyTower function.
    /// </summary>
    private void PlaceTower()
    {
        // WorldPosition locks placement within tiles.
        GameObject tower = Instantiate(GameManager.Instance.ClickedButton.TowerPrefab, currentTile.WorldPosition, Quaternion.identity);

        // Update layer
        //tower.GetComponent<Tower>().SetLayers(currentTile.Position.y); // WRONG gives negative layers

        // Use Tile limits[1].y is the lowest tile position value.
        // Its already negativ, so minus minus to get a positive variable.
        tower.GetComponent<Tower>().SetLayers(currentTile.Position.y -TileManager.Instance.TileLimits[1].y);

        // Set parent to tileMap GameObject, the object this script is on.
        tower.transform.SetParent(transform);

        GameManager.Instance.BuyTower();
    }
}
