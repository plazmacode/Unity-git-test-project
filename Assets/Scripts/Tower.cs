using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Enemy Target { get; private set; }

    private List<Enemy> enemies = new List<Enemy>();

    [SerializeField]
    private int moveDistance = 5;

    [SerializeField]
    private GameObject turret;

    [SerializeField]
    private string projectileType;
    [SerializeField]
    private float projectileSpeed = 5;

    [SerializeField]
    private int damage = 1;

    private bool canAttack = true;
    private float attackTimer;

    [SerializeField]
    private float attackCooldown;

    [SerializeField]
    private TextMeshProUGUI towerMoveCooldownText;

    [SerializeField]
    private SpriteRenderer rangeSpriteRenderer;

    [SerializeField]
    private CircleCollider2D rangeCollider;

    private List<Collider2D> overlaps = new List<Collider2D>();

    [SerializeField]
    private int towerMoveCooldown = 3;

    public int TowerMoveCooldown {
        get
        {
            return towerMoveCooldown;
        }
        set
        {
            towerMoveCooldown = value;
            towerMoveCooldownText.text = towerMoveCooldown.ToString();
        }
    }

    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public int Damage { get => damage; set => damage = value; }
    public int MoveDistance { get => moveDistance; set => moveDistance = value; }

    private void Awake()
    {
        GameManager.Instance.Towers.Add(this);
    }

    private void Update()
    {
        UpdateEnemies();
        Attack();
    }

    private void UpdateEnemies()
    {
        enemies = new List<Enemy>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.NoFilter();
        rangeCollider.OverlapCollider(contactFilter2D, overlaps);

        for (int i = 0; i < overlaps.Count; i++)
        {
            if (overlaps[i].tag == "Enemy")
            {
                enemies.Add(overlaps[i].GetComponent<Enemy>());
            }
        }
    }

    public void Select()
    {
        rangeSpriteRenderer.enabled = !rangeSpriteRenderer.enabled;
    }

    private void Rotate()
    {
        // Rotate speed version
        //turret.transform.rotation = Quaternion.RotateTowards(transform.rotation, Target.transform.rotation, 1);

        // Instant rotation
        turret.transform.up = Target.transform.position - transform.position;
    }

    private void Attack()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }

        if (enemies.Count > 0)
        {
            int index = enemies.Min(x => x.WaveIndex);
            Target = enemies.Find(x => x.WaveIndex == index);
        } else
        {
            Target = null;
        }

        // If there is no curent target, but other potential targets exists
        if (Target == null && enemies.Count > 0)
        {
            // Change target
        }

        if (Target != null && Target.IsActive)
        {
            Rotate();
            if (canAttack)
            {
                Shoot();

                canAttack = false;
            }
        }
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();
        projectile.Initizialize(this);
        projectile.transform.position = transform.position;
    }

    /// <summary>
    /// Updates layer of tower and turet
    /// </summary>
    public void SetLayers(int layer)
    {
        // BAD Performance, move GetComponent to start or use in-editor fields
        GetComponent<SpriteRenderer>().sortingOrder = layer;
        turret.GetComponent<SpriteRenderer>().sortingOrder = layer + 1;
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.tag == "Enemy")
    //    {
    //        Enemy e = other.GetComponent<Enemy>();
    //        enemies.Add(e);
    //        e.Towers.Add(this);
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.tag == "Enemy")
    //    {
    //        Enemy e = other.GetComponent<Enemy>();

    //        enemies.Remove(e);
    //        e.Towers.Remove(this);
    //    }
    //}
}
