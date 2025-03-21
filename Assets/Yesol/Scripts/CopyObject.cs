using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;

public class CopyObject : MonoBehaviour
{
    public GameObject copyObject; // 복제된 오브젝트
    public GameObject newObject;
    public float scale = 0.9f; // 복제할 오브젝트 크기
    public float speed = 2.0f; //돌아가는 속도

    private void OnMouseOver() // 마우스를 올려놓고 있을 때 
    {
        // 마우스 좌클릭 시
        if (Input.GetMouseButtonDown(0))
        {
            // 오브젝트 밑면 길이 : 오브젝트 바로 앞에 복제 오브젝트가 나오도록 
            float objectlength = gameObject.GetComponent<Renderer>().bounds.size.z;

            // 복제 오브젝트 스폰 위치 : 복제 오브젝트 위치 - 메인 카메라(플레이어 시점) 앞방향 * 오브젝트 밑면 길이
            Vector3 spawnpoint = gameObject.transform.position - Camera.main.transform.forward * objectlength;

            //복제 오브젝트 (복제할 대상, 복제 위치, 각도)
            newObject = Instantiate(copyObject, spawnpoint, Quaternion.identity);

            // 복제 오브젝트 크기 : 원본 * 0.9배
            newObject.transform.localScale = copyObject.transform.localScale * scale;

            newObject.tag = "Copy";

        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Copy");

            var sort = objects.OrderByDescending
                (obj => obj.transform.localScale.magnitude).ToList();

            foreach (GameObject obj in sort)
            {
                float moveSpeed = speed * obj.transform.localScale.magnitude;

                obj.transform.position = Vector3.MoveTowards
                (obj.transform.position, gameObject.transform.position,
                    Time.deltaTime * moveSpeed);

                if (Vector3.Distance(obj.transform.position, gameObject.transform.position) < 0.1f)
                {
                    Destroy(obj);
                }
            }
        }
    }
}
    



    