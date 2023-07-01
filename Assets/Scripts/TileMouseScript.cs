using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileMouseScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Tilemap tilemap;
    private Tile currentTile;
    private Tile previousTile;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2Int mousePosition = GetMousePosition();
        Tile tile;
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
            currentTile = tile;

            // --------- MOUSE OVER TILE--------- 
            MouseOverTile(tile);
        }

        // --------- MOUSE OVER NEW TILE --------- 
        if (tile != previousTile && tile != null)
        {
            TileManager.Instance.ColorTile(tile.Position, Color.white);
            previousTile = tile;
        }


        if (Input.GetMouseButtonDown(0))
        {
            //DebugText();
        }
    }
    private void MouseOverTile(Tile tile)
    {
        //if (!tilemap.GetSprite((Vector3Int)tile.Position))
        //{
        //    // Exit if tile has no sprite
        //    return;
        //}
        // If mouse not over UI and Tower button has been selected.
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedButton != null && tile != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GameManager.Instance.Money > GameManager.Instance.ClickedButton.Price)
                {
                    // subtract money
                    GameManager.Instance.Money -= GameManager.Instance.ClickedButton.Price;

                    // increase price by base price
                    GameManager.Instance.ClickedButton.Price += GameManager.Instance.ClickedButton.BasePrice;

                    // Place Tower sets ClickedButton to null, meaning variables from their will not be accessible anymore
                    PlaceTower();
                }
            }
        }
    }

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
            string pathStatus = currentTile.IsPath ? "is Path" : "is not Path";
            Debug.Log($"Pos:{currentTile.Position}: {walkableStatus}: {pathStatus}");
        }
        else
        {
            Debug.Log($"Pos{currentTile.Position}: Tile has no node");
        }
    }

    private void PlaceTower()
    {
        // WorldPosition locks placement within tiles.
        GameObject tower = Instantiate(GameManager.Instance.ClickedButton.TowerPrefab, currentTile.WorldPosition, Quaternion.identity);

        // Update layer
        tower.GetComponent<Tower>().SetLayers(currentTile.Position.y);

        // Set parent to tileMap GameObject, the object this script is on.
        tower.transform.SetParent(transform);

        GameManager.Instance.BuyTower();
    }
}
