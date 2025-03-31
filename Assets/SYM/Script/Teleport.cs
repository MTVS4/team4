using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleports : MonoBehaviour
{
    // 물리적 속성을 다룰 경우 Rigidbody 변수를 선언
    private Rigidbody rigidbody;
    private Vector3 velocity;
    public float yaw;
    private float smoothYaw;

    // 물리 기반 오브젝트일 경우, Rigidbody 초기화
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // 텔레포트 메서드
    public void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        // 위치 동기화
        transform.position = pos;

        // 회전 동기화
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;

        // 물리 기반 오브젝트일 경우, 속도 동기화
        if (rigidbody != null)
        {
            // 물리 속도 (Linear, Angular) 동기화
            rigidbody.linearVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(rigidbody.linearVelocity));
            rigidbody.angularVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(rigidbody.angularVelocity));
        }

        // 트랜스폼 동기화
        Physics.SyncTransforms();
    }
}
