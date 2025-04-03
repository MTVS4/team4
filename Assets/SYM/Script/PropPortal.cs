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

    // 🔹 4-6. 위치, 회전, 속도 동기화 (앞으로 들어가서 앞으로 나오는 방식)
    // fromPortal 기준 로컬 좌표로 변환
    Vector3 localPos = fromPortal.InverseTransformPoint(transform.position);
    Quaternion localRot = Quaternion.Inverse(fromPortal.rotation) * transform.rotation;
    Vector3 localVel = fromPortal.InverseTransformDirection(velocity);

    // X와 Z 좌표 반전 (왼쪽/오른쪽, 앞/뒤 반전)
    localPos.x = -localPos.x;
    localPos.z = -localPos.z;
    // 180도 회전 추가 (앞→앞 관계 구현)
    localRot = Quaternion.Euler(0, 180f, 0) * localRot;
    // 속도 방향도 반전
    localVel.x = -localVel.x;
    localVel.z = -localVel.z;

    // toPortal 기준으로 다시 변환
    transform.position = toPortal.TransformPoint(localPos);
    Quaternion newRot = toPortal.rotation * localRot;
    velocity = toPortal.TransformDirection(localVel);

    // Y축 회전만 적용 (Yaw)
    smoothYaw = newRot.eulerAngles.y;
    yaw = smoothYaw;
    transform.eulerAngles = Vector3.up * smoothYaw;

    // 🔹 7. 물리 엔진 동기화 (위치 및 회전 적용)
    Physics.SyncTransforms();
  }
}
