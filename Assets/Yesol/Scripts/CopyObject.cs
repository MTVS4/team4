using System.Collections;
using UnityEngine;

public class CopyObject : MonoBehaviour
{
    public GameObject originObject;
    public float scale = 0.9f; 
    public float speed = 2f;
    
    private void OnMouseOver()
    {
        // 마우스 좌클릭 -> 복제
        if (Input.GetMouseButtonDown(0))
        {
            // 오브젝트 밑변 "길이" (복제 오브젝트가 이전 오브젝트 앞에 생성되게 하기위해)
            float objectLength = GetComponent<Renderer>().bounds.size.x;
            // 복제 오브젝트 스폰 위치 = 현재 오브젝트 위치 - 카메라 앞 방향 * 이전 오브젝트 길이 
            Vector3 spawnPoint = transform.position - Camera.main.transform.forward * objectLength;
            
            // 현재 오브젝트 복제해서 새 오브젝트(복제할 오브젝트) 생성 
            GameObject newObj = Instantiate(gameObject, spawnPoint, Quaternion.identity);
            // 복제 오브젝트 크기 = 원본 오브젝트 * 0.9
            newObj.transform.localScale = transform.localScale * scale;
            // 복제 오브젝트 태그 설정 
            newObj.tag = "Copy";

            // 복제 오브젝트 스크립트 가져오기 
            CopyObject newScript = newObj.GetComponent<CopyObject>();
            
            // originObject 설정 : 복제 오브젝트이고 원본 오브젝트 정보가 있다면? originObject를 물려준다 : 자신을 원본으로 설정한다 
            newScript.originObject = (tag == "Copy" && originObject != null) ? originObject : this.gameObject;
        }

        // 우클릭 -> 복제 오브젝트 복귀하며 제거
        if (Input.GetMouseButtonDown(1))
        {
            // 원본 찾기 : 복제본이면? originObject 사용 : 자신이 원본 
            GameObject trueOrigin = (tag == "Copy" && originObject != null) ? originObject : this.gameObject;

            // 복제 오브젝트 찾기 
            GameObject[] allCopies = GameObject.FindGameObjectsWithTag("Copy");
            
            // 복제 오브젝트 반복 검사 
            foreach (GameObject copy in allCopies)
            {
                if (copy != null)
                {
                    // 복제 오브젝트 스크립트 참조 
                    CopyObject copyScript = copy.GetComponent<CopyObject>();
                    
                    // trueOrigin에서 파생된 복제본이면 
                    if (copyScript.originObject == trueOrigin)
                    {
                        // 물리 충돌 제거 (이동 중 충돌 방지)
                        copy.GetComponent<Collider>().enabled = false;
                        // 리지드바디 키네마틱 설정 (물리 영향 x)
                        copy.GetComponent<Rigidbody>().isKinematic = true;
                        
                        // 삭제 코루틴 호출
                        StartCoroutine(MoveDestroy(copy));
                    }
                }
            }
        }
    }

    IEnumerator MoveDestroy(GameObject clone)
    {
        // 복제 오브젝트 스크립트 참조 
        CopyObject cloneScript = clone.GetComponent<CopyObject>();

        // 목표 위치 = 원본 오브젝트의 현재 위치 
        Vector3 targetPos = cloneScript.originObject.transform.position;

        // 현재 위치, 목표 위치 가까워질 때까지 반복 
        while (Vector3.Distance(clone.transform.position, targetPos) > 0.01f)
        {
            // 현재 위치를 목표 위치로 이동 
            clone.transform.position = Vector3.Lerp(clone.transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        // 다시 한 번 위치 시키고 삭제 
        clone.transform.position = targetPos;
        Destroy(clone);
    }
}