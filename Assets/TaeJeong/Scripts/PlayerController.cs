using System;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    
    // 플레이어를 WASD로 이동시키고
    // 메인 카메라를 붙여서 1인칭으로 만들고
    // 카메라로 보는 시점으로 움직이게 만들고
    // 점프 구현하기
    
    // [SerializeField] private  :  값 변경이 굳이 필요없는 변수들은 private 처리를 하는데
    //                              그러면 엔진에서 수정을 못함
    //                              엔진에서 수치 조정하면서 테스트해보는게 편하니까
    //                              퍼블릭으로 바꾸진 않고 private 그대로 사용하면서 엔진에서 보이도록 하는 방법
    
    // 플레이어 이동속도
    [SerializeField] private float moveSpeed = 5f;
    
    // 마우스 감도
    public float mouseSensitivity = 500f;
    
    // 점프력
    [SerializeField] private float jumpForce = 5f;
    
    // 중력
    [SerializeField] private float gravity = -9.81f;
    
    // 캐릭터 컨트롤러를 참조할 변수
    private CharacterController characterController;
    
    // 메인 카메라의 Transform ( 플레이어 시점 )
    private Transform cameraTransform;
    
    // 카메라의 수직 회전 값을 저장해둘 변수
    private float xRotation = 0f;
    
    // 점프 및 중력 적용을 위한 수직 속도를 저장해둘 변수
    private float verticalVelocity = 0f;
    
    public AudioClip footstepSound;  // 걸음 효과음 클립
    public AudioClip landingSound;   // 착지 효과음 클립
    private AudioSource audioSource;
    private float footstepTimer;     // 걸음 소리 재생 간격 타이머
    public float footstepInterval = 0.5f; // 걸음 소리 간격

    // 착지 감지를 위한 이전 지면 상태
    private bool wasGrounded = true;
    private bool isJumping = false; 
    private float jumpStartTime = 0f;  
    private float fallTime = 0f;

    private void Awake()
    {
        // 에디터 실행 시 바로 게임뷰로 포커싱 되도록 해주는 코드 
#if UNITY_EDITOR
        var gameWindow = UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.GameView"));
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
        //캐릭터컨트롤러 받아오기
        characterController = GetComponent<CharacterController>();
        
        //1인칭 캐릭터니까 메인 카메라 플레이어에 받아오기
        cameraTransform = Camera.main.transform;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // 2D 사운드로 재생 (공간감 없이)
        audioSource.spatialBlend = 0f;
        
        //마우스 커서 중앙에 고정, 숨김처리
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        
        bool currentlyGrounded = characterController.isGrounded;
        
        // 지면과 충돌하는 거 방지
        // 캐릭터가 지면에 있고 , 수직 속도가 음수 ( 내려가는 중 ) 라면
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            // 약간의 하강 속도를 유지하여 안정적인 접촉을 유지하도록
            verticalVelocity = -2f;
        }

        // 플레이어 WASD 이동
        
        float horizontal = Input.GetAxis("Horizontal"); // 수평값
        float vertical = Input.GetAxis("Vertical"); // 수직값
        
        // 카메라의 forward 벡터를 가져와서 수평 성분만 사용하게 y값 제거
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize(); // 벡터 정규화
        
        // 똑같이 카메라의 right 벡터를 가져와 수평 성분만 사용하게 y값 제거
        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize(); // 벡터 정규화

        
        // 카메라 방향을 기준으로 이동 방향 결정
        Vector3 move = (camForward * vertical + camRight * horizontal);

        // 위 작업 이유 : 플레이어가 카메라로 보는 방향으로 이동을 해야해서
        //              이거 안해주면 보는 방향 무시하고 그냥 앞뒤양옆으로만 이동함
        //              y값을 0으로 설정하는 이유는 카메라가 기울어졌을 경우를 없애는 거
        
        
        // 대각선 이동 시 속도가 너무 빠르지 않도록 정규화
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }
        
        // "Jump" 버튼(기본 Space) 입력과 함께 지면에 있을 경우 점프
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
            isJumping = true;
            jumpStartTime = Time.time; 
        }

        // 중력 적용: 매 프레임마다 중력 값을 수직 속도에 더해 자연스러운 낙하 구현
        verticalVelocity += gravity * Time.deltaTime;
        
        // 이동 벡터에 현재의 수직 속도 반영 (점프 및 중력)
        move.y = verticalVelocity;
        
        // 이동 벡터에 이동 속도와 프레임 보정을 곱해주고
        move *= moveSpeed * Time.deltaTime;
        
        // 캐릭터 이동 처리
        characterController.Move(move);

        if (currentlyGrounded && move.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                if (footstepSound != null)
                {
                    audioSource.PlayOneShot(footstepSound);
                }
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        if (!currentlyGrounded)
        {
            fallTime += Time.deltaTime;
        }
        else
        {
            if (!wasGrounded && currentlyGrounded && isJumping)
            {
                if (landingSound != null)
                {
                    float jumpDuration = Time.time - jumpStartTime;  
                    float landingVolume = Mathf.Lerp(0.2f, 1.0f, Mathf.InverseLerp(0f, 2f, jumpDuration));
                    audioSource.PlayOneShot(landingSound, landingVolume);
                }
                isJumping = false;
            }

            if (fallTime > 0.2f && landingSound != null)
            {
                float landingVolume = Mathf.Lerp(0.2f, 1.0f, Mathf.InverseLerp(1.0f, 5.0f, fallTime));
                audioSource.PlayOneShot(landingSound, landingVolume);
            }

            fallTime = 0f;
        }

        // 현재 상태를 다음 프레임을 위해 저장
        wasGrounded = currentlyGrounded;

        // 마우스 입력을 통한 화면 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // 좌우
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // 상하

        // 플레이어를 기준으로 좌우 회전
        transform.Rotate(Vector3.up * mouseX);
        
        // 카메라의 상하 회전을 위해 수직 회전 값 누적 ( 마우스 Y 입력 반전 적용 )
        xRotation -= mouseY;
        
        // 상하 회전 각도를 -90° ~ 90° 사이로 제한하여 과도한 회전 방지
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        // 카메라의 로컬 회전을 업데이트
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

    }
}