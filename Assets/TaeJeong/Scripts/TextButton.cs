using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText; 
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color pressedColor = Color.gray;
    void Awake()
    {
        
        if (buttonText == null)
        {
            buttonText = GetComponent<TextMeshProUGUI>();
        }
        buttonText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        buttonText.color = pressedColor;
    }
}
