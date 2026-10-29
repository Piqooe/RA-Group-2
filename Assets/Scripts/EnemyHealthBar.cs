using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    private Health health;

    private void Awake()
    {
        health = GetComponentInParent<Health>();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnHealthChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void Start()
    {
        if (health != null)
        {
            UpdateFillAmount(health.HealthPercentage);
        }
    }

    private void HandleHealthChanged(float percentage)
    {
        UpdateFillAmount(percentage);
    }

    private void UpdateFillAmount(float percentage)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = percentage;
        }
    }
}
