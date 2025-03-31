using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperProp : MonoBehaviour
{
    public bool outline = true;
    public Color pointedColor = new(0f, 0.57f, 1f, 0f);
    public Color holdColor = new(1f, 1f, 1f, 1f);
    
    public Camera playerCamera;
    public PickResize playerPick;

    private Vector3 shakeScale = new(3f, 3f, 3f);
    private bool isShake = false;
    public float raycastDistance = 100f; 
    public LayerMask groundLayerMask;

    private bool isFalling = false;
    //카메라 쉐이크
    private float shakeTime;
    private float shakeIntensity;
    

    // 일정 스케일 이상 커지고 바닥에 닿으면
    // 플레이어 카메라 쉐이크 주기
    
    void Update()
    {
        
        FallingCheck();
        
       
    }
    
    void OnShakeCamera(float shakeTime = 1.0f, float shakeIntensity = 0.1f)
    {
        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;
        
        StopCoroutine("ShakeByPosition");
        StartCoroutine("ShakeByPosition");
    }

    IEnumerator ShakeByPosition()
    {
        Vector3 originalPosition = playerCamera.transform.position;

        while (shakeTime > 0.0f)
        {
            playerCamera.transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
            
            shakeTime -= Time.deltaTime;
            
            yield return null;
        }
        
        playerCamera.transform.position = originalPosition;
    }

    // 바닥에서
    void FallingCheck()
    {
        Collider col = GetComponent<Collider>();
        Vector3 rayStart = transform.position - new Vector3(0f, col.bounds.extents.y, 0f);

        RaycastHit hit;
        // 한 번의 레이캐스트로 지면과의 거리를 측정합니다.
        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastDistance, groundLayerMask))
        {
            float distance = hit.distance;
        
            // 떨어지고 있다고 판단하는 조건
            if (distance > 0.5f)
            {   
                Debug.Log("Falling");
                isFalling = true;
            }
            // 이전에 떨어지고 있었고, 이제 지면에 거의 닿았을 때
            else if (distance < 0.01f && isFalling && !playerPick.isPick)
            {
                    Debug.Log("Shaking");
                OnShakeCamera(0.4f, 0.1f);
                isFalling = false;
            }
        }
    }
    
    
    
    
    
}