using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public event Action<float> OnHealthChanged;

    public float HealthPercentage => (float)currentHealth / maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(gameObject.name + " HP: " + currentHealth);

        OnHealthChanged?.Invoke(HealthPercentage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (CompareTag("Enemy"))
        {
            LevelManager.instance.AddKill();
        }

        Destroy(gameObject);
    }
}
