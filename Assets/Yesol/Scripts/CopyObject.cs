using System.Collections;
using UnityEngine;

public class CopyObject : MonoBehaviour
{
    public static Vector3 originPosition; // 원본 위치를 모든 오브젝트가 알 수 있게 static으로 설정
    public static Quaternion originRotation;

    public float scale = 0.9f; // 복제되는 크기
    public float speed = 2f; // 사라지는 속도 

    void Update()
    {
        // 원본 오브젝트 위치 계속 업데이트 
        if (tag != "Copy")
        {
            originPosition = transform.position;
        }
    }

    private void OnMouseOver()
    {
        // 좌클릭 시 복제
        if (Input.GetMouseButtonDown(0))
        {
            // 복제된 오브젝트가 이전 오브젝트 앞에 생성되게 밑면 길이 구해주기 
            float objectLength = GetComponent<Renderer>().bounds.size.z;
            // 복제 오브젝트 스폰 위치 : 복제 오브젝트 위치 - 메인 카메라(플레이어 시점) 앞방향 * 오브젝트 밑면 길이
            Vector3 spawnPoint = transform.position - Camera.main.transform.forward * objectLength;
            
            //복제 오브젝트 (복제할 대상, 복제 위치, 각도) 
            GameObject newObj = Instantiate(gameObject, spawnPoint, Quaternion.identity);
            // 복제 오브젝트 크기 : 원본 * 0.9배
            newObj.transform.localScale = transform.localScale * scale;
            // 복제 오브젝트 태그 
            newObj.tag = "Copy";
        }

        // 우클릭 시 모든 복제 오브젝트를 원본 위치로 되돌림
        if (Input.GetMouseButtonDown(1))
        {
            // "Copy" 태그 오브젝트 배열로 저장
            GameObject[] copies = GameObject.FindGameObjectsWithTag("Copy");
            
            // copy 태그 오브젝트 = obj 하나씩 반복
            foreach (GameObject copy in copies)
            {
                // obj가 비어있지 않고
                if (copy != null)
                {
                    // 콜라이더 해제 
                    copy.GetComponent<Collider>().enabled = false;
                    
                    // 리지드바디 isKinematic 설정 (물리 영향 x)
                    copy.GetComponent<Rigidbody>().isKinematic = true;
                    
                    /*Rigidbody rb = copy.GetComponent<Rigidbody>();
                    if (rb != null) rb.isKinematic = true;*/

                    StartCoroutine(MoveBackAndDestroy(copy));
                }
            }
        }
    }

    IEnumerator MoveBackAndDestroy(GameObject clone) // clone은 복제된 오브젝트
    {
        while (Vector3.Distance(clone.transform.position, originPosition) > 0.01f)
        {
            // 복제 오브젝트 위치 원본 오브젝트로 
            clone.transform.position = Vector3.Lerp(clone.transform.position, originPosition, speed * Time.deltaTime);
            yield return null;
        }
        // 한 번 더 조정 
        clone.transform.position = originPosition;
        Destroy(clone);
    }
}

    