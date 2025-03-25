using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem.Controls;




public class TriggerboxText : MonoBehaviour
{
    public Transform playerObj2;
    public GameObject text;
    private float playerSpeed = 2f;
    
    private float x,z;

    public GameObject fakeDecal;
    private float truncatedx, truncatedz;



  // camera rotate
  [SerializeField]
    private float eulerAngX;
    [SerializeField]
    private float eulerAngY;
    [SerializeField]
    private float eulerAngZ;

    //  부동소수점 비교 오차 보정 변수
    private float pointPositonX = -1.04f, pointPositonZ = -6.01f;
    private float errorDistance =0.01f;


    void Awake()
    {
      
       text.SetActive(false);
       fakeDecal.SetActive(true);

    }

    void Update()
    {
        //  decalMat1.SetFloat("_power",  Alpha); 
        
        eulerAngX = playerObj2.transform.localEulerAngles.x;
        eulerAngY = playerObj2.transform.localEulerAngles.y;
        eulerAngZ = playerObj2.transform.localEulerAngles.z;
    // print(eulerAngX + " " + eulerAngY + " " + eulerAngZ);
    Debug.Log(playerObj2.transform.position);



  }
    private void OnTriggerStay(Collider Player)
    {
        // 트리거 박스 플레이어 위치 보정  
       x = playerObj2.transform.position.x;
       z = playerObj2.transform.position.z;
       truncatedx = Mathf.Floor(x * 100f) / 100f;
       truncatedz = Mathf.Floor(z * 100f) / 100f;

      

     
        //  데칼 끄고 큐브 키기
        if(Player.gameObject.tag =="Trigger")
        { 
          //  히트존으로 보정정
          playerObj2.transform.position += (transform.position - playerObj2.transform.position) * Time.deltaTime * playerSpeed;
        }

    if ( IsCloseEnough(x, pointPositonX) && IsCloseEnough(z, pointPositonZ) && 305f < eulerAngY &&  eulerAngY < 355)
    // if (truncatedx == -1.0f && truncatedz == -6.0f && 305f < eulerAngY &&  eulerAngY < 355)
    {
      print("ture");

     
              fakeDecal.SetActive(false);
              text.SetActive(true);
              Destroy(this.gameObject);
            }
            
    }

    private bool IsCloseEnough( float Trigger, float target)
    {
      return Mathf.Abs(Trigger - target) < errorDistance;
    } 

}








