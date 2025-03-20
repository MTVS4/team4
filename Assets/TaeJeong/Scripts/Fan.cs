using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 400;
    [SerializeField] private float windForce = 200;
    
    private Rigidbody rb;
    private GameObject wings;
    public PlayerController player;
    void Start()
    {
        wings = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody>();
    }

   
    void Update()
    {
        wings.transform.Rotate( Time.deltaTime * rotSpeed * Vector3.up);
    }

    void OnTriggerStay(Collider other)
    {
        // 팬의 바람에 영향을 받을 객체의 Rigidbody 가져오기
        Rigidbody otherRb = other.attachedRigidbody;
        
        if(otherRb != null)
        {
            // 팬의 forward 방향을 바람 방향으로 사용 (필요에 따라 조절 가능)
            Vector3 windDirection = -transform.forward;
            // 객체의 질량에 따라 힘을 조절하거나, 거리 보정 등을 추가할 수 있습니다.
            otherRb.AddForce(windDirection * windForce, ForceMode.Acceleration);
            
        }
    }
    
    
}
