using System.Collections;
using UnityEngine;

public class Copytest : MonoBehaviour
{

    public GameObject copyObject; // 복제된 오브젝트
    public float scale = 0.9f; // 복제할 오브젝트 크기
    public float speed = 2;

    void Start()
    {
        // 처음에 자기 자신 복제 
        copyObject = gameObject;
    }
    
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
            GameObject newObject = Instantiate(copyObject, spawnpoint, Quaternion.identity);

            // 복제 오브젝트 크기 : 원본 * 0.9배
            newObject.transform.localScale = copyObject.transform.localScale * scale;

            newObject.tag = "Copy";
            
        }

        
        if (Input.GetMouseButtonDown(1))
        {
            // "Copy" 태그를 가진 모든 객체들을 찾음
            GameObject[] copies = GameObject.FindGameObjectsWithTag("Copy");
            
            // copy 태그 오브젝트 = obj
            foreach (GameObject obj in copies)
            {
                //obj가 비어있지 않고 비교한 태그가 Original 이라면
                if(obj != null && gameObject.CompareTag("Original"))
                {
                    // 콜라이더 해제
                    obj.GetComponent<Collider>().enabled = false;
                    // 리지드바디 isKinematic 설정
                    obj.GetComponent<Rigidbody>().isKinematic = true;
                    // 코루틴 실행 obj를 매개변수로 넣어줌
                    StartCoroutine(MoveCopyToOriginal(obj));
                }
            }
        }
    }
    
    IEnumerator MoveCopyToOriginal(GameObject clone) // 받아서 사용할 매개변수 이름을 clone으로 지정
    {
        while (Vector3.Distance(clone.transform.position, copyObject.transform.position) > 0.01f)
        {
            // 회전값 맞춰주기
            clone.transform.rotation = copyObject.transform.rotation;
            // 러프 사용해서 위치 이동하기
            clone.transform.position = Vector3.Lerp(clone.transform.position, copyObject.transform.position, speed*Time.deltaTime);
            yield return null;
        }
        
        //러프 이후에 위치값을 정확하게 원본위치로 맞추기
        clone.transform.position = copyObject.transform.position;
        //삭제하기
        Destroy(clone);
        
    }
}


