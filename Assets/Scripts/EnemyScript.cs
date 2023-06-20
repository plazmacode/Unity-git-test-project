using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Stack<Node> path;

    public Vector2Int GridPosition { get; set; }

    private Vector3 destination;

    public void Start()
    {
        SetPath(TileManager.Instance.Path);
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
