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
        // If Left mouse down, tile is not null, mouse is not over a UI GameObject
        if (Input.GetMouseButtonDown(0) && tile != null && !EventSystem.current.IsPointerOverGameObject())
        {
            // The DebugCanvas blocks our tower placement
            TileManager.Instance.ClearDebugCanvas();

            // ---------- FIRST IF-STATEMENT FOR TOWER BUY/PLACEMENT------------
            // Checks if tower is being placed(a button is clicked)
            if (GameManager.Instance.ClickedButton != null)
            {
                // Check price and if their already is a tower.
                // Alternatively add a PlacementBlocked bool for more cases of a blocked tile in case of tower placement
                if (GameManager.Instance.Money > GameManager.Instance.ClickedButton.Price && !currentTile.HasTower)
                {
                    // subtract money
                    GameManager.Instance.Money -= GameManager.Instance.ClickedButton.Price;

                    // increase price by base price
                    GameManager.Instance.ClickedButton.Price += GameManager.Instance.ClickedButton.BasePrice;

                    // Place Tower sets ClickedButton to null, meaning variables from their will not be accessible anymore
                    PlaceTower(tile);

                    currentTile.HasTower = true;
                    currentTile.TileNode.SetWalkable(false); // Make enemies not walk over this tile.
                    // Implementation of this can be done differently later.

                    // Implementation 1: Use a HasTower bool with the AStar instead of only a Walkable bool

                    // Implementation 2: Set Walkable automatically when HasTower property is changed.
                }
                // Tower selection, if tower-button not clicked / not placing a tower.
            }
            else if (GameManager.Instance.ClickedButton == null)
            {
                if (GameManager.Instance.MovedTower != null)
                {
                    Debug.Log("Moving Tower to new Position.");
                    GameManager.Instance.MovedTower.gameObject.transform.position = tile.WorldPosition;
                    tile.HasTower = true;
                    tile.MyTower = GameManager.Instance.MovedTower;
                    GameManager.Instance.MovedTower = null;
                }
                //How do I double click inside this giant if-statement 💀
                if (Input.GetKey(KeyCode.LeftShift) && tile.MyTower != null)
                {
                    Debug.Log("Moving Tower. Click tile to place.");
                    GameManager.Instance.MovedTower = tile.MyTower;
                    tile.MyTower = null;
                    tile.HasTower = false;
                }

                if (tile.MyTower != null)
                {
                    GameManager.Instance.SelectTower(tile.MyTower);
                }
                else
                {
                    GameManager.Instance.DeselectTower();
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
    private void PlaceTower(TileValue tile)
    {
        // WorldPosition locks placement within tiles.
        GameObject tower = Instantiate(GameManager.Instance.ClickedButton.TowerPrefab, currentTile.WorldPosition, Quaternion.identity);

        tile.MyTower = tower.GetComponent<Tower>();

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
