using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead;

    public event Action<float> OnHealthChanged;

    public float HealthPercentage => (float)currentHealth / maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth -= damage;

        currentHealth -= damage;
        Debug.Log($"[Health] {gameObject.name} terkena {damage} damage. HP: {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(HealthPercentage);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (CompareTag("Enemy") && LevelManager.instance != null)
        {
            LevelManager.instance.AddKill();
        }

        Destroy(gameObject);
    }
}
