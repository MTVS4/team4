using UnityEngine;

public class PropPortal : PortalTraveller
{
  Vector3 velocity;
  public float yaw;
  float smoothYaw;
  
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

    // 🔹 4. 위치 이동 (포탈을 통해 새로운 위치로 텔레포트)
    transform.position = pos;

    // 🔹 5. 회전 동기화 (Yaw 회전값 보정)
    Vector3 eulerRot = rot.eulerAngles;
    float delta = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
    yaw += delta;
    smoothYaw += delta;
    transform.eulerAngles = Vector3.up * smoothYaw;

    // 🔹 6. 속도 벡터 변환 (포탈 회전 반영)
    velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));

    // 🔹 7. 물리 엔진 동기화 (위치 및 회전 적용)
    Physics.SyncTransforms();
  }
}
