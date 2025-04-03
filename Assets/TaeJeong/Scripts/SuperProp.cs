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

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    
    
    private void Update()
    {
        if (playerPick.isPick)
        {
            boxCollider.enabled = false;
        }
        else
        {
            boxCollider.enabled = true;
        }
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
    
    
    
    
    
}