using UnityEngine;

public class PropPortal : PortalTraveller
{
    
      // 위치동기화 변수 
    Vector3 velocity;
    public float yaw;
    float smoothYaw;
    // Update is called once per frame
    //  위치 동기화
      public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle (smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
        Physics.SyncTransforms ();
    }
}
