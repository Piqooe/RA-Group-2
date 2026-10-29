
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private int damage = 1;
    private float nextDamageTime = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            StopMovement();
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;

        rb.linearVelocity = direction * moveSpeed;

        if (animator != null)
        {
            float moveX = Mathf.Abs(direction.x) >= Mathf.Abs(direction.y) ? Mathf.Sign(direction.x) : 0f;
            float moveY = Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? Mathf.Sign(direction.y) : 0f;

            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveY", moveY);
            animator.SetBool("IsMoving", rb.linearVelocity.magnitude > 0.01f);
        }
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            Health health = collision.GetComponentInParent<Health>();

            if (health != null && health.CompareTag("Player"))
            {
                health.TakeDamage(damage);
                nextDamageTime = Time.time + 1f;
            }
        }
    }
    
}
