using UnityEngine;

public class PlayerControllerRb : MonoBehaviour
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


    /// 유니티 에디터에서 시작할 때 자동으로 커서가 사라지게 하는 코드
    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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
}
