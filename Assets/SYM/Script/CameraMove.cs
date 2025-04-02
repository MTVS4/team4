using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private Camera portalCamera;

    [SerializeField]
    private int iterations = 2;

    public Transform fromPortal;
    public Transform toPortal;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("메인 카메라가 할당되지 않았습니다!", this);
        }
        if (portalCamera == null)
        {
            Debug.LogError("portalCamera가 할당되지 않았습니다!", this);
        }
    }

    private void Update()
    {
        for (int i = iterations - 1; i >= 0; --i)
        {
            UpdateCameraPosition(fromPortal, toPortal, i);
            UpdateCameraPosition(toPortal, fromPortal, i);
        }
    }

    private void UpdateCameraPosition(Transform inTransform, Transform outTransform, int iterationID)
    {
        Transform cameraTransform = portalCamera.transform;

        // 카메라 위치 초기화
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= iterationID; ++i)
        {
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        if (mainCamera != null)
        {
            var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            portalCamera.projectionMatrix = newMatrix;
        }
        else
        {
            Debug.LogWarning("메인 카메라가 존재하지 않습니다. Oblique Frustum을 설정할 수 없습니다.");
        }
    }
}