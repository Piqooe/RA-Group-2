using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image fillImage;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<Health>();
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void Start()
    {
        UpdateFillAmount(playerHealth != null ? playerHealth.HealthPercentage : 1f);
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
