using System;
using Unity.VisualScripting;
using UnityEngine;

public class Button2 : MonoBehaviour
{
    public GameObject door;  
    public float slideDistance = 5f;  // 문이 움직이는 거리
    public float slideSpeed = 2f;  // 문이 움직이는 속도

    private bool isOpened = false;  // 문이 열려있는지
    private Vector3 closedPosition;  // 문이 닫힌 위치
    private Vector3 openedPosition;  // 문이 열린 위치

    void Start()
    {
    closedPosition = door.transform.position;
    openedPosition = closedPosition - new Vector3(slideDistance, 0, 0);
    }

    private void OnTriggerStay(Collider other)
    {
        isOpened = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isOpened = false;
    }


    void Update()
    {
        door.transform.position = 
            Vector3.Lerp(door.transform.position, isOpened ? openedPosition : closedPosition, slideSpeed * Time.deltaTime );
        Debug.Log(openedPosition);
    }
}
