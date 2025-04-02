using System;
using UnityEngine;

public class ButtonPressed : MonoBehaviour
{
    public Transform model;
    private Vector3 originalPosition; 
    private Vector3 targetPosition;   
    public float pressDepth = 0.95f;   
    public float speed = 5f;          
    private bool isPressed = false; 
    

    void Start()
    {
        originalPosition = model.transform.position;
        targetPosition = originalPosition; // 초기 목표 위치 설정
    }

    void OnTriggerStay(Collider other)
    {
        isPressed = true;
        targetPosition = originalPosition - new Vector3(0, pressDepth, 0);
    }

    void OnTriggerExit(Collider other)
    {
        // 버튼에서 물체가 떠났을 때만 원래 위치로 복귀
        isPressed = false;
        targetPosition = originalPosition;
    }

    void Update()
    {
        model.localPosition = Vector3.Lerp(model.localPosition, targetPosition, speed * Time.deltaTime);
    }
}