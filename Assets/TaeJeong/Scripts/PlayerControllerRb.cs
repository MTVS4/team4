using System.Collections;
using UnityEngine;

public class PlayerControllerRb : PortalTraveller
{
    private Rigidbody playerRb;
    public float mouseSensitivity = 100f;
    private float xRotation;
    private Camera cam;
    private Transform cameraTransform;
    [SerializeField] private GameManager gameManager;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;         
    [SerializeField] private float acceleration = 10f;      // 가속도
    [SerializeField] private float deceleration = 20f;      // 감속도
    [SerializeField] private float jumpForce = 8f;
    private bool isGrounded = true;
    private bool isFinish;

    // 위치동기화 변수 
    Vector3 velocity;
    public float yaw;
    float smoothYaw;
    private bool isTeleporting = false;




    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    /// 유니티 에디터에서 시작할 때 자동으로 커서가 사라지게 하는 코드
#if UNITY_EDITOR
        var gameWindow =
            UnityEditor.EditorWindow.GetWindow(
                typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.GameView"));
        gameWindow.Focus();
        gameWindow.SendEvent(new Event
        {
            button = 0,
            clickCount = 1,
            type = EventType.MouseDown,
            mousePosition = gameWindow.rootVisualElement.contentRect.center
        });
#endif
    }

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.freezeRotation = true;
        cameraTransform = Camera.main.transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isFinish = false;
    }

    private void Update()
    {
        MouseMovement();
        if (!isFinish)
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                Jump();
            }
        }
    }

    

    private void FixedUpdate()
    {
        if (!isFinish)
        {
            PlayerMovement();
        }
    }
    
    void MouseMovement()
    {
        // 마우스 입력에 따른 회전 처리
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 플레이어 좌우 회전
        transform.Rotate(Vector3.up * mouseX);

        // 카메라 상하 회전 (마우스 Y는 반전)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical);
        inputDir = transform.TransformDirection(inputDir);

        if (inputDir.sqrMagnitude > 0.01f)
        {
            Vector3 targetVelocity = inputDir.normalized * maxSpeed;
            Vector3 currentVelocity = playerRb.linearVelocity;
            currentVelocity.y = 0f;

            Vector3 velocityChange = targetVelocity - currentVelocity;

            float maxVelocityChange = acceleration * Time.fixedDeltaTime;
            velocityChange = Vector3.ClampMagnitude(velocityChange, maxVelocityChange);

            playerRb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {
            Vector3 horizontalVelocity = playerRb.linearVelocity;
            horizontalVelocity.y = 0f;
            
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            
            playerRb.linearVelocity = new Vector3(horizontalVelocity.x, playerRb.linearVelocity.y, horizontalVelocity.z);
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        // 점프 입력 처리
        
    }

    void Jump()
    {
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Finish"))
        {
            Debug.Log("Finish");    
            gameManager.GameFinish();
            isFinish = true;
            gameManager.isFinish = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }
    }


    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot, Vector3 fromPortalScale, Vector3 toPortalScale)
    {
        if (isTeleporting) return;
        isTeleporting = true;

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

        // 🔹 4. "앞→앞" 포탈 방식으로 위치 계산 (거울 효과 적용)
        // fromPortal 기준 로컬 좌표로 변환 (정확한 거울 반사 효과를 위해)
        Vector3 localPos = fromPortal.InverseTransformPoint(transform.position);

        // X축만 반전 (왼쪽/오른쪽 반전)
        localPos.x = -localPos.x;

        // Z축은 부호를 그대로 유지 (앞뒤 관계 유지)
        // localPos.z = localPos.z;

        // toPortal 기준으로 다시 월드 좌표로 변환
        Vector3 newPos = toPortal.TransformPoint(localPos);

        // 포탈 앞으로 약간 오프셋 추가 (걸리는 현상 방지)
        newPos += toPortal.forward * 0.3f;

        // 🔹 5. 위치 이동
        transform.position = newPos;

        // 🔹 6. 회전 조절 - 거울 반사 효과 적용
        // fromPortal 기준 로컬 회전으로 변환
        Quaternion localRot = Quaternion.Inverse(fromPortal.rotation) * transform.rotation;

        // Y축 180도 회전 (앞→앞 관계)
        localRot = Quaternion.Euler(0, 180, 0) * localRot;

        // toPortal 기준으로 다시 월드 회전으로 변환
        Quaternion newRot = toPortal.rotation * localRot;

        // Y축 회전만 적용 (Yaw)
        smoothYaw = newRot.eulerAngles.y;
        yaw = smoothYaw;
        transform.eulerAngles = Vector3.up * smoothYaw;

        // 🔹 7. 속도 벡터 변환 (거울 반사 효과 적용)
        Vector3 localVel = fromPortal.InverseTransformDirection(velocity);
        // X축만 반전 (왼쪽/오른쪽 반전)
        localVel.x = -localVel.x;
        // Z축은 유지 (앞뒤 관계 유지)
        velocity = toPortal.TransformDirection(localVel);

        // 기존 속도의 일부를 유지하여 자연스러운 이동감 제공
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = velocity * 0.8f; // 80% 속도 유지
        }

        // 🔹 8. 물리 엔진 동기화
        Physics.SyncTransforms();

        // 잠시 후 텔레포트 상태 해제
        StartCoroutine(ResetTeleportState());
    }

    private IEnumerator ResetTeleportState()
    {
        yield return new WaitForSeconds(0.2f);
        isTeleporting = false;
    }
}
