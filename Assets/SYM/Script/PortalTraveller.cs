using UnityEngine;

public class PortalTraveller : MonoBehaviour {
    // protected로 변경하여 자식 클래스에서 접근할 수 있도록 합니다.
    protected GameObject graphicsObject;
    public GameObject graphicsClone { get; set; }
    public Vector3 previousOffsetFromPortal { get; set; }

    public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        transform.rotation = rot;
    }

    public virtual void EnterPortalThreshold() {
        if (graphicsClone == null) {
            graphicsClone = Instantiate(graphicsObject);
            graphicsClone.transform.parent = graphicsObject.transform.parent;
            graphicsClone.transform.localScale = graphicsObject.transform.localScale;
        } else {
            graphicsClone.SetActive(true);
        }
    }

    public virtual void ExitPortalThreshold() {
        graphicsClone.SetActive(false);
    }
}
