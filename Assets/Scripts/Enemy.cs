using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;

    private Stack<Node> path;

    public Vector2Int GridPosition { get; set; }

    private Vector3 destination;

    public void Spawn()
    {
        // Set start position from cell start from LevelManager Property

        transform.position = TileManager.Instance.TileMap.CellToWorld((Vector3Int)LevelManager.Instance.StartPosition);
    }

    public void Start()
    {
        SetPath(LevelManager.Instance.Path);
    }

    public void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (transform.position == destination)
        {
            if (path != null && path.Count > 0)
            {
                GridPosition = path.Peek().Position;
                destination = path.Pop().Tile.WorldPosition;
            }
        }
    }

    private void SetPath(Stack<Node> newPath)
    {
        if (newPath != null)
        {
            path = newPath;
            GridPosition = path.Peek().Position;
            destination = path.Pop().Tile.WorldPosition;
        }
    }
}
