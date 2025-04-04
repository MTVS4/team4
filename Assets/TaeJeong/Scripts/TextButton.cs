using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI buttonText;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color pressedColor = Color.gray;

    public AudioClip hoverSoundClip;
    private AudioSource audioSource;

    void Awake()
    {
        if (buttonText == null)
        {
            buttonText = GetComponent<TextMeshProUGUI>();
        }
        buttonText.color = normalColor;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = highlightColor;

        if (hoverSoundClip != null)
        {
            audioSource.PlayOneShot(hoverSoundClip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Text Clicked!");
        buttonText.color = pressedColor;
    }
}