using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem.Controls;




public class Triggerbox : MonoBehaviour
{
  public GameObject decal1;
  public Transform playerObj;
  public GameObject cube;
  public float playerSpeed = 2f;
  private float x, z;
  private float truncatedx, truncatedz;
  // DecalProjector cubeDecal;

  //  부동소수점 비교 오차 보정 변수
  // private float pointPositonX = 0.51f, pointPositonZ = 1.04f; 원래 좌표
  private float pointPositonX = -257.48f, pointPositonZ = -15.95f;
  private float errorDistance = 0.01f;

  public CharacterController characterController;


  // camera rotate
  [SerializeField] private float eulerAngX;
  [SerializeField] private float eulerAngY;
  [SerializeField] private float eulerAngZ;



  void Awake()
  {
    //  cubeDecal = decal1.GetComponent<DecalProjector>();
    cube.SetActive(false);
    decal1.SetActive(true);


  }

  void Update()
  {
    //  decalMat1.SetFloat("_power",  Alpha); 

    eulerAngX = playerObj.transform.localEulerAngles.x;
    eulerAngY = playerObj.transform.localEulerAngles.y;
    eulerAngZ = playerObj.transform.localEulerAngles.z;
    // print(eulerAngX + " " + eulerAngY + " " + eulerAngZ);


  }
  /*private void OnTriggerStay(Collider Player)
  {
      // 트리거 박스 플레이어 위치 보정
     x = playerObj.transform.position.x;
     z = playerObj.transform.position.z;
     truncatedx = Mathf.Floor(x * 100f) / 100f;
     truncatedz = Mathf.Floor(z * 100f) / 100f;

      //  데칼 끄고 큐브 키기
      if(Player.gameObject.tag =="Trigger")
      {
        //  히트존으로 보정정
        playerObj.transform.position += (transform.position - playerObj.transform.position) * Time.deltaTime * playerSpeed;


      }

          if ( IsCloseEnough(x, pointPositonX) && IsCloseEnough(z, pointPositonZ) && 155 < eulerAngY && eulerAngY < 205)
          {
            print("ture");
            cube.SetActive(true);
            decal1.SetActive(false);
            Destroy(this.gameObject);
          }

  }

private bool IsCloseEnough(float Trigger, float target)
{
  return Mathf.Abs(Trigger - target) < errorDistance;
}

}*/

  private void OnTriggerStay(Collider other)
  {
    if (other.gameObject.CompareTag("Trigger"))
    {
      Vector3 targetPos = transform.position;
      Vector3 currentPos = playerObj.position;
      Vector3 direction = (targetPos - currentPos).normalized;
      float distance = Vector3.Distance(currentPos, targetPos);

      // 이동할 거리를 계산합니다. 만약 목표와의 거리가 playerSpeed * Time.deltaTime보다 작으면 목표 위치까지 이동
      float step = Mathf.Min(playerSpeed * Time.deltaTime, distance);

      // CharacterController.Move()를 사용하여 이동
      characterController.Move(direction * step);
    }

    // 특정 조건 충족 시 처리
    if (IsCloseEnough(playerObj.position.x, pointPositonX) &&
        IsCloseEnough(playerObj.position.z, pointPositonZ) &&
        155 < eulerAngY && eulerAngY < 205)
    {
      Debug.Log("ture");
      cube.SetActive(true);
      decal1.SetActive(false);
      Destroy(this.gameObject);
    }
  }

  private bool IsCloseEnough(float current, float target)
  {
    return Mathf.Abs(current - target) < errorDistance;
  }
}





