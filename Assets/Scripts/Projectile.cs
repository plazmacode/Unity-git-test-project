using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private Tower parent;

    [SerializeField]
    private bool IsRotating;

    public void Initizialize(Tower parent)
    {
        this.parent = parent;
        target = parent.Target;
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
        Rotate();
    }

    private void Rotate()
    {
        Vector2 direction = target.transform.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void MoveToTarget()
    {
        if (target != null && target.IsActive)
        {
            // Move to target
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * parent.ProjectileSpeed);
        }
        else if (!target.IsActive)
        {
            // Destroy if target destroyed
            // Change implementation when needed depending on projectile type
            GameManager.Instance.Pool.ReleaseObject(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            if (target.gameObject == other.gameObject)
            {
                target.TakeDamage(parent.Damage);
                GameManager.Instance.Pool.ReleaseObject(gameObject);
            }
        }
    }
}
