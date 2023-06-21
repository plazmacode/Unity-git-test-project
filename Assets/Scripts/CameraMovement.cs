using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float cameraSpeed = 5;

    // Camerabounds
    // Different from minimum and maximum Tile value from TileManager
    // Contains minimum and maximum world position in order to make camerabounds
    private Vector2 minPosition = Vector2.zero;
    private Vector2 maxPosition = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        float tmpSpeed = cameraSpeed * Time.deltaTime;
        Vector2 vertical = Input.GetAxisRaw("Vertical") < 0 ? Vector2.down * tmpSpeed : Vector2.up * tmpSpeed;
        Vector2 horizontal = Input.GetAxisRaw("Horizontal") < 0 ? Vector2.left * tmpSpeed : Vector2.right * tmpSpeed;
        if (Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("Vertical") > 0)
        {
            transform.Translate(vertical);
        }
        if (Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.Translate(horizontal);
        }

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minPosition.x, maxPosition.x),
            Mathf.Clamp(transform.position.y, maxPosition.y, minPosition.y),
            transform.position.z
        );
    }


    public void SetLimits()
    {
        Tilemap tileMap = TileManager.Instance.TileMap;

        Vector3 minTilePosition = tileMap.CellToWorld((Vector3Int)TileManager.Instance.TileLimits[0]);
        Vector3 maxTilePosition = tileMap.CellToWorld((Vector3Int)TileManager.Instance.TileLimits[1]);

        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        // Remember the y position is negative when the camera moves down.
        // Therefore the mathClamp in GetInput() switches the max and minimum position of y.
        // Therefore the minimum position is minus the cameraHeight and maximum is plus.
        minPosition = new Vector2(minTilePosition.x + cameraWidth / 2, minTilePosition.y - cameraHeight / 2);
        maxPosition = new Vector2(maxTilePosition.x - cameraWidth / 2, maxTilePosition.y + cameraHeight / 2);

        // Camerabounds might be off by one, but it works right now because the Tile limits are also off by one :)
    }
}
