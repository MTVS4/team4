using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private Camera portalCamera;
    public Transform fromPortal;
    public Transform toPortal;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
      UpdateCameraPosition(fromPortal, toPortal);   
    }

    private void UpdateCameraPosition(Transform inTransform, Transform outTransform)
    {
        Transform portalCameraTransform = portalCamera.transform;

        // 카메라 위치 초기화
        portalCameraTransform.position = transform.position;
        portalCameraTransform.rotation = transform.rotation;
    
        Vector3 relativePos = inTransform.InverseTransformPoint(portalCameraTransform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        portalCameraTransform.position = outTransform.TransformPoint(relativePos);

        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * portalCameraTransform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        portalCameraTransform.rotation = outTransform.rotation * relativeRot;
        

        // view frustum 생성

        Vector3 toCamera = portalCamera.transform.position - outTransform.position;
        // Vector3 toCamera = outTransform.position - portalCameraTransform.position;
        // Plane p = new Plane( toCamera, outTransform.position);
        Vector3 aa = outTransform.position - new Vector3(0,0,-1);
        //  Plane p = new Plane( Vector3.Dot(toCamera, outTransform.forward) < 0 ? outTransform.forward : -outTransform.forward, outTransform.position);
        Plane p = new Plane( outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);

        
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

    

        
       
    }
}