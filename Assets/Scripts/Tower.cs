using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Enemy Target { get; private set; }

    private Queue<Enemy> enemies = new Queue<Enemy>();
    
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

    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public int Damage { get => damage; set => damage = value; }

    private void Awake()
    {
        // Moved to SerializedField
        //turret = GameObject.Find("Turret");
    }

    private void Update()
    {
        Attack();
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


        // If there is no curent target, but other potential targets exists
        if (Target == null && enemies.Count > 0)
        {
            // Change target
            Target = enemies.Dequeue();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            enemies.Enqueue(other.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Target = null;
        }
    }
}
