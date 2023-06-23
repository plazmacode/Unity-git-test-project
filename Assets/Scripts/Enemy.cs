using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;

    [SerializeField]
    private int originalHealth = 5;
    private int health;

    public bool IsActive { get; set; }
    private Stack<Node> path;
    public Vector2Int GridPosition { get; set; }

    private Vector3 destination;

    private GameObject healthBar;

    private void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(health / 10f, 0.05f, 1);
    }

    public void Spawn()
    {
        // Set start position from cell start from LevelManager Property
        transform.position = TileManager.Instance.TileMap.CellToWorld((Vector3Int)LevelManager.Instance.StartPosition);
        IsActive = true;
        SetPath(LevelManager.Instance.Path);
        health = originalHealth;
        UpdateHealthBar();
    }

    private void Awake()
    {
        healthBar = transform.Find("HealthBar").gameObject;
    }

    public void Update()
    {
        Move();
    }

    private void Move()
    {
        if (IsActive)
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

    private void Release()
    {
        IsActive = false;
        // Maybe add code for resetting position to start
        GameManager.Instance.Pool.ReleaseObject(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Release();
        }
        UpdateHealthBar();
    }
}
