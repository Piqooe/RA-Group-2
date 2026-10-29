
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead;

    public bool IsAlive => !isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        currentHealth -= damage;
        Debug.Log($"[Health] {gameObject.name} terkena {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"[Health] {gameObject.name} mati (HP habis).", gameObject);

        if (CompareTag("Enemy") && LevelManager.instance != null)
        {
            LevelManager.instance.AddKill();
        }

        Destroy(gameObject);
    }
}
