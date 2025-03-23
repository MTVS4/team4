using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem.Controls;




public class TriggerboxText : MonoBehaviour
{
    public Transform playerObj2;
    public GameObject text;
    public float playerSpeed2 = 1f;
    
    float x,z;

  public GameObject fakeDecal;
  
   
    
    // camera rotate
    [SerializeField]
    float eulerAngX;
    [SerializeField]
    float eulerAngY;
    [SerializeField]
    float eulerAngZ;

    

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
    private void OnTriggerStay(Collider Player2)
    {
        // 트리거 박스 플레이어 위치 보정  
       x = playerObj2.transform.position.x;
       z = playerObj2.transform.position.z;
       float truncatedx = Mathf.Floor(x * 100f) / 100f;
       float truncatedz = Mathf.Floor(z * 100f) / 100f;

      

     
        //  데칼 끄고 큐브 키기
        if(Player2.gameObject.tag =="Trigger")
        { 
          //  히트존으로 보정정
          playerObj2.transform.position += (transform.position - playerObj2.transform.position) * Time.deltaTime * playerSpeed2;
        }

    if (Mathf.Approximately(truncatedx, -1.04f) && Mathf.Approximately(truncatedz, -6.01f) && 305f < eulerAngY &&  eulerAngY < 355)
    // if (truncatedx == -1.04f && truncatedz == -6.01f && 305f < eulerAngY &&  eulerAngY < 355)
    {
      print("ture");

     
              fakeDecal.SetActive(false);
              text.SetActive(true);
              Destroy(this.gameObject);
            }
            
    }

}








