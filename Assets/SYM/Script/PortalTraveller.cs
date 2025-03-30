using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    public Vector3 previousOffsetFromPortal { get; set; } 
    // 외부에서 프로퍼티에 접근해서 읽을때 호출하는게 GET
    // 외부에서 프로퍼티를 접근해서 값을 할당할때(값을 변경시킬때 - 외부에서) SET 
    // GET OR SET 하나씩만 사용해서 읽거나 받거나 하나만 가능
    // { get; set; } 단순히 할당 반환만 할때

    public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot, Vector3 fromPortalScale, Vector3 toPortalScale)
    {
        transform.position = pos;
        transform.rotation = rot;
       
    }

}
