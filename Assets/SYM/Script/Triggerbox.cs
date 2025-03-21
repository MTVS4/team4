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
    public float playerSpeed = 1f;
    float x,z;
    public float Alpha;
    public float decalAlpha;
    public Material decalMat1;
    DecalProjector cubeDecal;
    public float fadeSpeed;
    
    // camera rotate
    [SerializeField]
    float eulerAngX;
    [SerializeField]
    float eulerAngY;
    [SerializeField]
    float eulerAngZ;

    

    void Awake()
    {
       cubeDecal = decal1.GetComponent<DecalProjector>();
       cube.SetActive(false);
       decal1.SetActive(true);

       Alpha = 1;
       decalAlpha = 1;
       
    }

    void Update()
    {
        //  decalMat1.SetFloat("_power",  Alpha); 
        
        eulerAngX = playerObj.transform.localEulerAngles.x;
        eulerAngY = playerObj.transform.localEulerAngles.y;
        eulerAngZ = playerObj.transform.localEulerAngles.z;
        print(eulerAngX + " " + eulerAngY + " " + eulerAngZ);
       
         
    }
    private void OnTriggerStay(Collider Player)
    {
        // 트리거 박스 플레이어 위치 보정  
       x = playerObj.transform.position.x;
       z = playerObj.transform.position.z;
       float truncatedx = Mathf.Floor(x * 100f) / 100f;
       float truncatedz = Mathf.Floor(z * 100f) / 100f;
 
        //  데칼 끄고 큐브 키기
        if(playerObj.gameObject.tag =="Trigger")
        { 
          //  히트존으로 보정정
          playerObj.transform.position += (transform.position - playerObj.transform.position) * Time.deltaTime * playerSpeed;
        }
            
            if (Mathf.Approximately(truncatedx, 0.51f) && Mathf.Approximately(truncatedz, 1.04f) && 155 < eulerAngY && eulerAngY < 205)
            {
              print("ture"); 
          
              cube.SetActive(true);
              decal1.SetActive(false);
              Destroy(this.gameObject);
            }
            
    }

}





