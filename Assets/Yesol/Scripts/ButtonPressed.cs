using System;
using UnityEngine;

public class ButtonPressed : MonoBehaviour
{
    private Vector3 originalPosition; // 버튼 원래 위치
    private Vector3 targetPosition;   // 버튼이 이동할 목표 위치
    public float pressDepth = 0.95f;   // 버튼이 눌리는 깊이
    public float speed = 5f;          // 이동 속도
    private bool isPressed = false;   // 버튼이 현재 눌려 있는지 확인

    void Start()
    {
        originalPosition = transform.position;
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
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }
}