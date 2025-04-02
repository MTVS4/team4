using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0f; // 흔들림 지속 시간
    public float shakeMagnitude = 0.1f; // 흔들림 강도
    private Vector3 originalPos; // 원래 위치
    public Transform playerCamera;
    void Start()
    {
        originalPos = playerCamera.localPosition; 
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            playerCamera.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime; 

        }
        else
        {
            playerCamera.localPosition = originalPos; 
        }
    }

    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
