using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControllerRb : MonoBehaviour
{
   private Rigidbody playerRb;
   
   public float mouseSensitivity;
   private float xRotation;
   private Camera cam;
   private Transform cameraTransform;
   [SerializeField] private float moveSpeed = 5f;
   [SerializeField] private float jumpForce = 5f;
   private bool isGrounded = true;

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
      playerRb = GetComponent<Rigidbody>();
      playerRb.freezeRotation = true;
      //1인칭 캐릭터니까 메인 카메라 플레이어에 받아오기
      cameraTransform = Camera.main.transform;
        
      //마우스 커서 중앙에 고정, 숨김처리
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.Locked;
   }

   void Update()
   {
      float horizontal = Input.GetAxis("Horizontal"); // 수평값
      float vertical = Input.GetAxis("Vertical"); // 수직값
      
      Vector3 moveVec = transform.forward * vertical + transform.right * horizontal;
      
      transform.position +=  moveSpeed * Time.deltaTime * moveVec.normalized;
      
      
      
      
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

      isGrounded = Physics.Raycast(transform.position, Vector3.down, 2 * 0.5f + 0.2f);

      if (Input.GetButtonDown("Jump") && isGrounded)
      {
         Jump();
      }
      
   }

   void Jump()
   { 
      playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
      isGrounded = false;
   }
}
