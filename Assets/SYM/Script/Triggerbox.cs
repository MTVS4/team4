using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem.Controls;




public class Triggerbox : MonoBehaviour
{
    public GameObject decal1;
    public Transform playerObj;
    public float playerSpeed = 1f;
    float x,z;
    public float Alpha;
    public float decalAlpha;
   
    public Material decalMat1;
   
    DecalProjector cubeDecal;
    public float fadeSpeed;

    public GameObject cube;

    

    void Awake()
    {
       cubeDecal = decal1.GetComponent<DecalProjector>();
       cube.GetComponent<MeshRenderer>();
       cube.SetActive(false);
       decal1.SetActive(true);

       Alpha = 1;
       decalAlpha = 1;
       
    }

    void Update()
    {
        //  decalMat1.SetFloat("_power",  Alpha); 
        //  cubeDecal.material.SetFloat("_Alpha",  decalAlpha);
         
    }
    private void OnTriggerStay(Collider Player)
    {
        // 트리거 박스 플레이어 위치 보정  
       x = playerObj.transform.position.x;
       z = playerObj.transform.position.z;
       float truncatedx = Mathf.Floor(x * 10f) / 10f;
       float truncatedz = Mathf.Floor(z * 10f) / 10f;
       print(truncatedx);
       print(truncatedz);
        
        if(Player.gameObject.tag =="Trigger")
        {
            Debug.Log("stay");
          playerObj.transform.position += (transform.position - playerObj.transform.position) * Time.deltaTime * playerSpeed;
        }
            
            if (Mathf.Approximately(truncatedx, -0.1f) && Mathf.Approximately(truncatedz, 3.0f))
            {
              print("ture"); 
            //   while(decalAlpha > 0f)
            //   {
            //     decalAlpha -= 0.1f * fadeSpeed * Time.deltaTime;
            //   }
            // decalAlpha = 0f;
            cube.SetActive(true);
            decal1.SetActive(false);
            }
            
    }

}
