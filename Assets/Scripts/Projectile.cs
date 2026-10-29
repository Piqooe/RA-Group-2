
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 1;
    public int pierce = 0;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Health health = collision.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }

            if (pierce <= 0)
                Destroy(gameObject);
            else
                pierce--;
        }
    }

}
