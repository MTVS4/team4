using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem.Controls;

public class TriggerPawn : MonoBehaviour
{
  public GameObject fakeDecal;
  public Transform playerObj;
  public GameObject pawn;

  private float playerSpeed = 2f;
  private float x, z;
  private float truncatedx, truncatedz;

  //  부동소수점 비교 오차 보정 변수
  // private float pointPositonX = 0.51f, pointPositonZ = 1.04f; 원래 좌표
  private float pointPositonX = 268.57f, pointPositonZ = 158.36f;
  private float errorDistance = 0.01f;

  [SerializeField]
  private float eulerAngX;
  [SerializeField]
  private float eulerAngY;
  [SerializeField]
  private float eulerAngZ;

  void Awake()
  {
    pawn.SetActive(false);
    fakeDecal.SetActive(true); 
  }

  void Update()
  {
    eulerAngX = playerObj.transform.localEulerAngles.x;
    eulerAngY = playerObj.transform.localEulerAngles.y;
    eulerAngZ = playerObj.transform.localEulerAngles.z;
  }
  private void OnTriggerStay(Collider Player)
  {
    // 트리거 박스 플레이어 위치 보정  
    x = playerObj.transform.position.x;
    z = playerObj.transform.position.z;
    truncatedx = Mathf.Floor(x * 100f) / 100f;
    truncatedz = Mathf.Floor(z * 100f) / 100f;

    
    if (Player.gameObject.tag == "Trigger")
    {
      //  히트존으로 보정정
      playerObj.transform.position += (transform.position - playerObj.transform.position) * Time.deltaTime * playerSpeed;
    }

    if (IsCloseEnough(x, pointPositonX) && IsCloseEnough(z, pointPositonZ) && 68 < eulerAngY && eulerAngY < 118)
    {
      // print("ture");
      pawn.SetActive(true);
      fakeDecal.SetActive(false);
      Destroy(this.gameObject);
    }

  }

  private bool IsCloseEnough(float Trigger, float target)
  {
    return Mathf.Abs(Trigger - target) < errorDistance;
  }

}





