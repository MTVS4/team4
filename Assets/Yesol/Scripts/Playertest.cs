using UnityEngine;

public class Playertest : MonoBehaviour
{
    // SerializeField : private인데 엔진에서 수정할 수 있도록 해줌
    
    // 이동속도
    [SerializeField] private float moveSpeed = 5f;
    
    // 마우스 감도
    [SerializeField] private float mouseSensitivity = 500;
    
    // 점프력
    [SerializeField] private float jumpForce = 5f;
    
    // 중력
    [SerializeField] private float gravity = -9.81f;
    
    // 캐릭터 컨트롤러를 참조하겠다
    private CharacterController characterController;
    
    // 메인 카메라 Transform 
    private Transform cameraTransform;
    
    // 카메라 수직 회전 값 (X축을 기준으로 수직 회전하니까)
    private float xRotation = 0f;
    
    // 점프, 중력 적용을 위한 수직 속도 저장
    private float verticalVelocity = 0f;
    
    private void Start()
    {
        // 캐릭터 컨트롤러 받기
        characterController = GetComponent<CharacterController>();
        
        // 메인 카메라 받기
        cameraTransform = Camera.main.transform;
        
        // 마우스 커서 중앙 고정, 숨김
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {
        // 지면에 플레이어 충돌을 방지하는 코드
        // 만약 캐릭터가 바닥에 있고, 수직 속도가 음수 (내려가는 중)
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            // 하강 속도를 유지하여 안정적인 접촉을 유지
            verticalVelocity = -2f;
        }
        
        float horizontal = Input.GetAxis("Horizontal"); // 수평
        float vertical = Input.GetAxis("Vertical"); // 수직
        
        // 카메라 앞(뒤) 벡터 가져와서 수직 y 제거 :  카메라 기우는 현상 방지
        Vector3 camForward = cameraTransform.forward; 
        camForward.y = 0;
        camForward.Normalize();
        
        // 카메라 좌우 벡터 가져와서 수직 y 제거
        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();
        
        // 카메라 방향을 기준으로 이동한다
        Vector3 move = (camForward * vertical) + (camRight * horizontal);

        // 대각선 이동 정규화
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }
        
        // 점프 버튼 입력하고, 지면에 있으면 점프한다
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
        
        // 매 프레임마다 중력값을 수직 속도에 더해서 자연스럽게 낙하
        verticalVelocity += gravity * Time.deltaTime;
        
        // 수직 이동 벡터에 현재 수직 속도 반영 
        move.y = verticalVelocity;
        
        // 이동 벡터에 이동속도 * 프레임 보정
        move *= moveSpeed * Time.deltaTime;
        
        // 캐릭터 이동 처리
        characterController.Move(move);
        
        // 마우스 입력을 통한 화면 회전 
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // 좌우
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // 상하
        
        // 플레이어 기준으로 좌우 회전 
        transform.Rotate(Vector3.up * mouseX);
        
        // 상하 회전을 위해 수직 회전 값 누적 (유니티는 아래로 내려가는 게 양수여서 마이너스 해줌)
        xRotation -= mouseY;
        
        // 상하 회전 각도 -90 ~ 90
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        // 카메라 로컬 회전 업데이트
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
