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
    //public GameObject playerObj;        // 플레이어 충돌 오브젝트 참조
    public Button button;               // 버튼 참조

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
    private PlayerControllerRb pcs;     // 플레이어 컨트롤러 스크립트 참조
    private float initialSens;          // 플레이어의 초기 마우스 민감도 저장
    private Vector3 startDirectionOffset; // 픽업 시 플레이어 forward와 대상 방향 차이 (보간 시작 오프셋)
    private float lerpStart;            // 보간 시작 시간 기록
    private bool isLerping;             // 대상이 보간 중인지 여부
    private int originalLayer;          // 픽업 전 대상의 레이어 저장
    private Quaternion rotOffset;       // 회전 오프셋 (현재 코드에서는 사용하지 않음)
    const float pi = 3.141592653589793238f; // 파이 상수

    // 초기 설정: 플레이어 컨트롤러와 마우스 민감도 저장
    void Start()
    {
        pcs = player.GetComponent<PlayerControllerRb>();
        initialSens = pcs.mouseSensitivity;
    }

    // 매 프레임 입력 처리
    void Update()
    {
        HandleInput();
    }

    // 모든 Update 후 대상의 위치와 스케일을 조정
    void LateUpdate()
    {
        ResizeTarget();
    }

    /// <summary>
    /// HandleInput: 입력에 따라 대상 픽업 및 드롭, 회전 처리를 합니다.
    /// </summary>
    void HandleInput()
    {
        // 왼쪽 마우스 버튼 클릭 시
        if (Input.GetMouseButtonDown(0))
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
                
                // 픽업 시작: 보간 시작 시간 기록 및 대상 할당
                lerpStart = Time.time;
                target = targetHit.transform;

                // 대상과 플레이어, 버튼 간 충돌 무시 설정
                Physics.IgnoreCollision(target.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(target.GetComponent<Collider>(), button.GetComponent<Collider>(), true);

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
            }
            // 이미 대상이 픽업된 상태이면 드롭(해제) 처리
            else
            {
                target.GetComponent<Rigidbody>().isKinematic = false;
                Physics.IgnoreCollision(target.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
                Physics.IgnoreCollision(target.GetComponent<Collider>(), button.GetComponent<Collider>(), false);
                pcs.mouseSensitivity = initialSens;
                target.gameObject.layer = originalLayer;
                target = null;
            }
        }

        // 오른쪽 마우스 버튼 입력 시 대상 회전 처리
        if (Input.GetMouseButton(1) && target != null)
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

    /// <summary>
    /// OutSine: 보간 진행 정도를 부드럽게 조절하는 이징 함수
    /// </summary>
    /// <param name="x">진행 비율 (0 ~ 1)</param>
    /// <returns>조절된 진행 비율</returns>
    private float OutSine(float x)
    {
        return (x > 1f) ? 1f : Mathf.Sin(0.5f * pi * x);
    }

    /// <summary>
    /// ResizeTarget: 픽업된 대상의 위치와 크기를 업데이트합니다.
    /// </summary>
    private void ResizeTarget()
    {
        if (target == null)
            return;

        // 플레이어의 forward 방향으로 BoxCast를 통해 대상의 새 위치 계산
        Vector3 targetPos = BoxCast(transform.forward);
        
        if (isLerping)
        {
            // 보간 진행 비율 계산
            float lerpPercentage = (Time.time - lerpStart) / lerpTime;
            if (lerpPercentage > 1)
            {
                lerpPercentage = 1;
                isLerping = false; // 보간 완료 시 보간 종료
            }
            // 시작 오프셋을 반영한 시작 위치 계산
            Vector3 startPos = BoxCast(transform.forward + startDirectionOffset);
            float progress = OutSine(lerpPercentage);
            target.position = Vector3.Lerp(startPos, targetPos, progress);
        }
        else
        {
            target.position = targetPos;
        }
        // 플레이어와의 거리에 따라 대상 스케일 업데이트
        target.localScale = CalcScale(target.position, originalSize);
    }

    /// <summary>
    /// CalcScale: 플레이어와 대상 간의 거리 비례로 대상의 스케일을 계산합니다.
    /// </summary>
    /// <param name="tarPos">대상의 현재 위치</param>
    /// <param name="originalSize">대상의 원래 로컬 스케일</param>
    /// <returns>계산된 스케일</returns>
    private Vector3 CalcScale(Vector3 tarPos, Vector3 originalSize)
    {
        float currentDistance = Vector3.Distance(transform.position, tarPos);
        float scale = currentDistance / originalDistance;
        return scale * originalSize;
    }

    /// <summary>
    /// RotateObj: 오른쪽 마우스 버튼 입력에 따라 대상의 회전을 조작합니다.
    /// </summary>
    private void RotateObj()
    {
        // 회전 중에는 플레이어의 마우스 민감도를 0으로 하여 카메라 회전 영향 배제
        pcs.mouseSensitivity = 0;
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * initialSens * 10f;
        target.localEulerAngles = new Vector3(target.localEulerAngles.x,
                                               target.localEulerAngles.y - mouseX,
                                               target.localEulerAngles.z);
    }

    /// <summary>
    /// BoxCast: OverlapBox를 사용해 플레이어 forward 방향상의 적절한 대상 위치를 찾습니다.
    /// 먼저 전진하며 충돌이 발생한 지점을 찾고, 이후 역방향으로 이동하여 충돌이 해소되는 경계 위치를 반환합니다.
    /// </summary>
    /// <param name="Direction">플레이어 forward 또는 오프셋 포함 방향</param>
    /// <returns>계산된 위치</returns>
    private Vector3 BoxCast(Vector3 Direction)
    {
        // 초기 박스 중심: 플레이어 위치에서 Direction 방향으로 0.5만큼 떨어진 위치
        Vector3 boxCenter = Direction * 0.5f + transform.position;
        Vector3 boxSize;
        Collider[] collider = new Collider[0];

        // 전진: 충돌(Collider)이 감지될 때까지 이동
        while (collider.Length == 0)
        {
            boxCenter += Direction * (1f / sample);
            boxSize = CalcScale(boxCenter, originalBoundSize) * 0.5f;
            collider = Physics.OverlapBox(boxCenter, boxSize, target.rotation, collisionMask);
            // 최대 거리 초과 시 최대 거리 위치 반환
            if (Vector3.Distance(transform.position, boxCenter) > maxScaleDistance)
                return Direction * maxScaleDistance + transform.position;
        }
        // 역진: 충돌 상태에서 겹침이 해소되는 경계 위치 찾기
        while (collider.Length != 0)
        {
            boxCenter -= Direction * (1f / sample);
            boxSize = CalcScale(boxCenter, originalBoundSize) * 0.5f;
            collider = Physics.OverlapBox(boxCenter, boxSize, target.rotation, collisionMask);
        }
        return boxCenter;
    }
}
