using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour
{
    public GameObject door;  
    public float slideDistance = 5f;  // 문이 움직이는 거리
    public float slideSpeed = 2f;  // 문이 움직이는 속도

    private bool isOpened = false;  // 문이 열려있는지
    private Vector3 closedPosition;  // 문이 닫힌 위치
    private Vector3 openedPosition;  // 문이 열린 위치
    private Coroutine moveCoroutine; // 현재 실행 중인 코루틴
    

    void Start()
    {
        // 초기 위치 저장
        closedPosition = door.transform.position;
        openedPosition = closedPosition - new Vector3(slideDistance, 0, 0);
    }

    void OnTriggerStay(Collider col) //트리거 충돌 시 실행 함수 
    {
        if (!isOpened) //문이 열려있지 않으면 (닫혀있으면) 
        {
            isOpened = true; // 한번 실행하고 끝남 (다시 버튼을 눌러도 중복 실행 방지)
            StartMovingDoor(openedPosition); // 문을 열기 시작 : 코루틴 실행 
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (isOpened) //문이 열려있으면 
        {
            isOpened = false;
            StartMovingDoor(closedPosition); // 문을 닫기 시작 : 코루틴 실행 
        }
    }

    void StartMovingDoor(Vector3 targetPosition) // 문 바로바로 이동
    {
        // 현재 진행 중인 코루틴이 비어있지 않으면 (코루틴이 실행 중이면) 정지시킨다 -> 기존 움직임 정지하고 새 움직임 시작 
        if (moveCoroutine != null) 
        {
            StopCoroutine(moveCoroutine);
        }
        
        // 새롭게 이동 코루틴 실행
        moveCoroutine = StartCoroutine(MoveDoor(targetPosition));
    }
    
    

    IEnumerator MoveDoor(Vector3 targetPosition) // 문 목표 위치로 이동하는 코루틴 
    {
        // 문이 목표 위치로 거의 도달할 때까지 반복
        while (Vector3.Distance(door.transform.position, targetPosition) > 0.01f) 
        {
            // Lerp로 부드럽게 이동 : (시작, 끝, 시간)
            door.transform.position = Vector3.Lerp(door.transform.position, targetPosition, slideSpeed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기 (프레임마다 반복 실행)
        }

        // 최종 위치 보정 (Lerp의 미세한 오차 방지)
        door.transform.position = targetPosition;   
    }
}


