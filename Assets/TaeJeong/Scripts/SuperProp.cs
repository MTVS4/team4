using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperProp : MonoBehaviour
{
    public bool outline = true;
    public Color pointedColor = new(0f, 0.57f, 1f, 0f);
    public Color holdColor = new(1f, 1f, 1f, 1f);
    public CameraShake cameraShake;
    public PickResize playerPick;
    
    public AudioClip objectDropClip;

    private BoxCollider boxCollider;
    private MeshCollider meshCollider;
    
    
    private ButtonPressed currentButtonUnderneath;
    private bool wasColliderEnabled = true;
    private Button currentButton;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        meshCollider = GetComponent<MeshCollider>();
    }
    
    
    private void Update()
    {
        bool isHeld = playerPick.target == transform;

        if (boxCollider != null)
            boxCollider.enabled = !isHeld;

        if (meshCollider != null)
            meshCollider.enabled = !isHeld;

        // 👇 콜라이더 꺼졌는지 감지하는 부분!
        bool isColliderNowDisabled = (boxCollider != null && !boxCollider.enabled) ||
                                     (meshCollider != null && !meshCollider.enabled);

        if (wasColliderEnabled && isColliderNowDisabled)
        {
            // 콜라이더가 방금 꺼졌으면 → 버튼에게 직접 알림!
            if (currentButtonUnderneath != null)
            {
                currentButtonUnderneath.ForceExit(GetComponent<Collider>());
                currentButtonUnderneath = null;
            }
            if (currentButton != null)
            {
                currentButton.ForceExit(GetComponent<Collider>());
                currentButton = null;
            }
        }

        wasColliderEnabled = !isColliderNowDisabled;
    }

    void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;
        if(impactForce > 4)
        {
            // 충돌한 물체의 위치에서 소리 재생 (3D 감쇠 효과 적용)
            AudioSource.PlayClipAtPoint(objectDropClip, transform.position);
            if (cameraShake != null && transform.localScale.sqrMagnitude > 27)
            {
                cameraShake.TriggerShake(0.1f, 0.1f); // 
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        ButtonPressed button = other.GetComponent<ButtonPressed>();
        if (button != null)
        {
            currentButtonUnderneath = button;
        }
        Button btn = other.GetComponent<Button>();
        if (btn != null)
        {
            currentButton = btn;
        }
    }
    
    
    
    
    
}