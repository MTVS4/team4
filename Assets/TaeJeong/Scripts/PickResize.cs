using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PickResize는 플레이어가 대상 오브젝트를 픽업하여 이동, 크기 조절, 회전할 수 있도록 하는 스크립트입니다.
/// 대상 픽업 시 해당 오브젝트와 플레이어, 버튼 간 충돌을 무시하고, 픽업 후에는 대상의 위치와 스케일을
/// 플레이어와의 거리에 따라 조절합니다.
/// </summary>
public class PickResize : MonoBehaviour
{
    // 플레이어 관련 참조
    public GameObject player;           // 플레이어 게임오브젝트 참조
    public Button[] buttons;            // 버튼 참조
    public GameManager gameManager;

    // 레이어 및 거리 설정
    public LayerMask targetMask;        // 픽업 대상 레이어
    public LayerMask collisionMask;     // 충돌 검사에 사용할 레이어
    public int enabledLayer;            // 픽업 후 대상의 레이어로 변경할 값
    [Range(50, 1000)]
    public float maxScaleDistance;      // 대상의 크기 및 위치 계산 시 고려할 최대 거리
    [Range(8, 256)]
    public int sample;                  // BoxCast 시 위치를 찾기 위해 사용되는 샘플 횟수
    public bool enableLerp;             // 대상 이동 시 보간(lerp) 사용 여부
    [Range(0, 5)]
    public float lerpTime;              // 보간 진행 시간

    // 상태 저장 및 내부 변수
    [HideInInspector]
    public Transform target;            // 현재 픽업되어 조작 중인 대상의 Transform
    float originalDistance;             // 픽업 시 플레이어와 대상 간 초기 거리
    Vector3 originalSize;               // 대상의 원래 로컬 스케일
    Vector3 originalBoundSize;          // 대상 Collider의 bounds 사이즈 (순서: x, z, y)
    float originalYRotation;            // 픽업 시 대상과 플레이어 간의 y축 회전 차이
    private PlayerController pcs;     // 플레이어 컨트롤러 스크립트 참조
    private float initialSens;          // 플레이어의 초기 마우스 민감도 저장
    private Vector3 startDirectionOffset; // 픽업 시 플레이어 forward와 대상 방향 차이 (보간 시작 오프셋)
    private float lerpStart;            // 보간 시작 시간 기록
    private bool isLerping;             // 대상이 보간 중인지 여부
    private int originalLayer;          // 픽업 전 대상의 레이어 저장
    private Quaternion rotOffset;       // 회전 오프셋 (현재 코드에서는 사용하지 않음)
    const float pi = 3.141592653589793238f; // 파이 상수

    public bool isPick;                 // BlockDoor에서 사용할 물체 잡았는지 확인하는 변수
    public bool isOverlapDoor;
    
    public AudioClip pickupSoundClip;  // 픽업 시 재생할 효과음 클립
    public AudioClip dropSoundClip;    // 드롭 시 재생할 효과음 클립
    private AudioSource audioSource;   
    private Collider[] tempColliders = new Collider[100];
    
    // 초기 설정: 플레이어 컨트롤러와 마우스 민감도 저장
    void Start()
    {
        pcs = player.GetComponent<PlayerController>();
        initialSens = pcs.mouseSensitivity;
        audioSource = GetComponent<AudioSource>();
    }

    // 매 프레임 입력 처리
    void Update()
    {
        if (!isOverlapDoor)
        {
            HandleInput();
        }
    }

    // 모든 Update 후 대상의 위치와 스케일을 조정
    void LateUpdate()
    {
        ResizeTarget();
    }


    // HandleInput: 입력에 따라 대상 픽업 및 드롭, 회전 처리를 합니다.
    public void HandleInput()
    {
        // 왼쪽 마우스 버튼 클릭 시
        if (Input.GetMouseButtonDown(0) && gameManager.isPlaying)
        {
            // 대상이 아직 픽업되지 않은 경우
            if (target == null)
            {
                // "Ground" 레이어에 대한 Raycast로 벽이나 바닥을 먼저 탐지하여 대상 선택 방지
                int groundLayer = LayerMask.NameToLayer("Ground");
                int groundLayerMask = 1 << groundLayer;
                RaycastHit groundHit;
                bool hitGround = Physics.Raycast(transform.position, transform.forward, out groundHit, maxScaleDistance, groundLayerMask);
                
                // 픽업 대상 Raycast (targetMask 사용)
                RaycastHit targetHit;
                bool hitTarget = Physics.Raycast(transform.position, transform.forward, out targetHit, maxScaleDistance, targetMask);
                if (!hitTarget)
                    return;
                
                // 만약 지면이 대상보다 먼저 충돌하면 픽업 취소
                if (hitGround && groundHit.distance < targetHit.distance)
                    return;
                
                if (audioSource != null && pickupSoundClip != null)
                {
                    audioSource.PlayOneShot(pickupSoundClip);
                }
                
                // 픽업 시작: 보간 시작 시간 기록 및 대상 할당
                lerpStart = Time.time;
                target = targetHit.transform;

                // 대상과 플레이어, 버튼 간 충돌 무시 설정
                Physics.IgnoreCollision(target.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
                foreach (Button btn in buttons)
                {
                    Collider btnCol = btn.GetComponent<Collider>();
                    if (btnCol != null)
                    {
                        Physics.IgnoreCollision(target.GetComponent<Collider>(), btnCol, true);
                    }
                }

                // 픽업 시 플레이어와 대상 사이의 방향 오프셋 계산
                startDirectionOffset = (target.position - transform.position).normalized - transform.forward;
                // 대상 Rigidbody를 kinematic으로 전환하여 물리 영향 배제
                target.GetComponent<Rigidbody>().isKinematic = true;
                // 픽업 시 플레이어와 대상 간의 초기 거리 및 스케일 저장
                originalDistance = Vector3.Distance(transform.position, target.position);
                originalSize = target.localScale;

                // 대상 Collider의 bounds 사이즈 저장 (x, z, y 순서로 재배열)
                var bound = target.GetComponent<Collider>().bounds.size;
                originalBoundSize = new Vector3(bound.x, bound.z, bound.y);
                // 픽업 시점의 y축 회전 차이 저장
                originalYRotation = target.localEulerAngles.y - transform.localEulerAngles.y;
                // 대상의 원래 레이어를 저장한 후 픽업 후 레이어로 변경
                originalLayer = target.gameObject.layer;
                target.gameObject.layer = enabledLayer;
                // 보간(lerp) 적용 여부 설정
                isLerping = enableLerp;
                
                // 오브젝트 잡았음
                isPick = true;
            }
            // 이미 대상이 픽업된 상태이면 드롭(해제) 처리
            else
            {
                
                if (audioSource != null && dropSoundClip != null)
                {
                    audioSource.PlayOneShot(dropSoundClip);
                }
                target.GetComponent<Rigidbody>().isKinematic = false;
                Physics.IgnoreCollision(target.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
                foreach (Button btn in buttons)
                {
                    Collider btnCol = btn.GetComponent<Collider>();
                    if (btnCol != null)
                    {
                        Physics.IgnoreCollision(target.GetComponent<Collider>(), btnCol, false);
                    }
                }

                pcs.mouseSensitivity = initialSens;
                target.gameObject.layer = originalLayer;
                target = null;
                
                // 오브젝트 놓았음
                isPick = false;
            }
        }

        // 오른쪽 마우스 버튼 입력 시 대상 회전 처리
        if (Input.GetMouseButton(1) && target != null && gameManager.isPlaying)
        {
            RotateObj();
        }
        else if (target != null)
        {
            // 회전 입력이 없으면 플레이어의 마우스 민감도 복원
            pcs.mouseSensitivity = initialSens;
        }

        // 오른쪽 마우스 버튼 해제 시 회전 기준값 재설정
        if (Input.GetMouseButtonUp(1) && target != null)
        {
            originalYRotation = target.localEulerAngles.y - transform.localEulerAngles.y;
        }
    }

    private float OutSine(float x)
    {
        return (x > 1f) ? 1f : Mathf.Sin(0.5f * pi * x);
    }
    
    private void ResizeTarget()
    {
        if (target == null || !isPick) // 오브젝트를 놓은 후에는 크기 조정 안 함
            return;
        
        Vector3 targetPos = BoxCastPosition(transform.forward);
    
        if (isLerping)
        {
            float lerpPercentage = (Time.time - lerpStart) / lerpTime;
            if (lerpPercentage > 1)
            {
                lerpPercentage = 1;
                isLerping = false; 
            }
            Vector3 startPos = BoxCastPosition(transform.forward + startDirectionOffset);
            float progress = OutSine(lerpPercentage);
            target.position = Vector3.Lerp(startPos, targetPos, progress);
        }
        else
        {
            target.position = targetPos;
        }
    
        // 플레이어와의 거리에 따라 대상 스케일 업데이트 (단, isPick이 true일 때만)
        if (isPick)
        {
            target.localScale = CalcScale(target.position, originalSize);
        }
    }


    
    // 새로운 스케일 = (현재 거리 / 초기 거리) × 원래 스케일
    private Vector3 CalcScale(Vector3 tarPos, Vector3 originalSize)
    {
        float currentDistance = Vector3.Distance(transform.position, tarPos);
        float scale = currentDistance / originalDistance;
        return scale * originalSize;
    }

    // 오브젝트 회전 시키기
    private void RotateObj()
    {
        // 회전 중에는 플레이어의 마우스 민감도를 0으로 하여 카메라 회전 영향 배제
        pcs.mouseSensitivity = 0;
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * initialSens * 10f;
        target.localEulerAngles = new Vector3(target.localEulerAngles.x,
                                               target.localEulerAngles.y - mouseX,
                                               target.localEulerAngles.z);
    }

    // 박스캐스트로 장애물 보다 앞으로 오도록
    private Vector3 BoxCastPosition(Vector3 direction)
    {
        Vector3 origin = transform.position;
        Quaternion rotation = target.rotation;
        Vector3 halfExtents = originalBoundSize * 0.5f;
        float safeMargin = 0.001f; // 진짜 미세하게 줄임
        float minDistance = 0.1f; // 플레이어와 너무 겹치지 않도록 최소 거리 확보
        float maxStep = 0.005f; // 한 스텝마다 얼마나 미세하게 이동할지
        int maxAttempts = 30;   // 정밀도 높이려면 반복 횟수 늘리기

        float finalDistance = maxScaleDistance;

        // Step 1. 콜리전까지의 거리 측정
        if (Physics.BoxCast(origin, halfExtents, direction, out RaycastHit hit, rotation, maxScaleDistance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            finalDistance = Mathf.Max(hit.distance - safeMargin, minDistance);
        }

        // Step 2. 해당 위치에서 실제 겹치는지 확인 + 미세 전진
        Vector3 candidate = origin + direction.normalized * finalDistance;

        for (int i = 0; i < maxAttempts; i++)
        {
            int count = Physics.OverlapBoxNonAlloc(candidate, halfExtents - Vector3.one * safeMargin, tempColliders, rotation, collisionMask, QueryTriggerInteraction.Ignore);

            if (count == 0)
                break; // 완벽! 겹치지 않음

            candidate += direction.normalized * maxStep;
        }

        return candidate;
    }









    
    
}
