
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 1;
    public int pierce = 0;
    private readonly HashSet<Health> hitEnemies = new HashSet<Health>();

    private void Start()
    {
        ScheduleDestruction();
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }

    public void Initialize(int projectileDamage, float projectileSpeed, float projectileLifetime, int projectilePierce)
    {
        damage = Mathf.Max(0, projectileDamage);
        speed = Mathf.Max(0f, projectileSpeed);
        lifeTime = Mathf.Max(0f, projectileLifetime);
        pierce = Mathf.Max(0, projectilePierce);
        ScheduleDestruction();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health health = collision.GetComponentInParent<Health>();
        if (health == null || !health.IsAlive || !health.CompareTag("Enemy") || hitEnemies.Contains(health)) return;

        hitEnemies.Add(health);
        health.TakeDamage(damage);

        if (pierce <= 0)
            Destroy(gameObject);
        else
            pierce--;
    }

    private void ScheduleDestruction()
    {
        CancelInvoke(nameof(SelfDestruct));
        Invoke(nameof(SelfDestruct), lifeTime);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
