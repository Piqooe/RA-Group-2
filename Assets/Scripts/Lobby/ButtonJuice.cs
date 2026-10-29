using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    [SerializeField] private Vector3 pressScale = new Vector3(0.9f, 0.9f, 1f);
    [SerializeField] private float animationSpeed = 10f;

    private Vector3 targetScale = Vector3.one;
    private Vector3 initialScale;

    private void Start()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = initialScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = pressScale;
    }
}
