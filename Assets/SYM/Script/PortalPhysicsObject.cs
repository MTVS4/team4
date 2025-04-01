using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PortalPhysicsObject : PortalTraveller
{
    new Rigidbody rigidbody;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot, Vector3 fromPortalScale, Vector3 toPortalScale)
    {
        // 🔹 1. 기존 크기 저장
        Vector3 originalScale = transform.localScale;

        // 🔹 2. 피벗을 "바닥 중앙"으로 맞추기 위한 위치 보정 (크기 변경 전에 적용)
        float heightBefore = originalScale.y;  // 변경 전 높이
        float heightAfter = heightBefore * (toPortalScale.y / fromPortalScale.y); // 변경 후 예상 높이
        float heightDifference = (heightAfter - heightBefore) / 2f; // 높이 변화의 절반

        // 위치 보정 (현재 피벗이 중앙이므로 Y축 기준으로 이동)
        transform.position += new Vector3(0, heightDifference, 0);

        // 🔹 3. 스케일 동기화 (포탈 간 크기 비율 유지)
        transform.localScale = new Vector3(
            originalScale.x * (toPortalScale.x / fromPortalScale.x),
            originalScale.y * (toPortalScale.y / fromPortalScale.y),
            originalScale.z * (toPortalScale.z / fromPortalScale.z)
        );
        
        base.Teleport(fromPortal, toPortal, pos, rot, fromPortalScale, toPortalScale);
        rigidbody.linearVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(rigidbody.linearVelocity));
        rigidbody.angularVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(rigidbody.angularVelocity));
    }
}