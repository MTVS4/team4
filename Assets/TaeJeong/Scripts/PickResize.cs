using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PickResize는 플레이어가 대상 오브젝트를 픽업하여 이동, 크기 조절, 회전할 수 있도록 하는 스크립트입니다.
/// Superliminal 스타일처럼 대상이 벽과 겹치지 않도록 Viewport 기반 Raycast 방식으로 처리됩니다.
/// </summary>
public class PickResize : MonoBehaviour
{
    public GameObject player;
    public Button button;
    public LayerMask targetMask;
    public LayerMask collisionMask;
    public int enabledLayer;
    [Range(50, 1000)]
    public float maxScaleDistance;
    public bool enableLerp;
    [Range(0, 5)]
    public float lerpTime;

    [SerializeField] private int gridRows = 6;
    [SerializeField] private int gridCols = 6;

    public Transform target;
    private Vector3 originalSize;
    private float originalDistance;
    private float originalYRotation;
    private Vector3 originalViewportPos;
    private PlayerController pcs;
    private float initialSens;
    private Vector3 startDirectionOffset;
    private float lerpStart;
    private bool isLerping;
    private int originalLayer;
    private Renderer targetRenderer;
    private List<Vector3> shapedGrid = new List<Vector3>();

    public bool isPick;
    public bool isOverlapDoor;

    public AudioClip pickupSoundClip;
    public AudioClip dropSoundClip;
    private AudioSource audioSource;
    private Camera playerCamera;

    void Start()
    {
        pcs = player.GetComponent<PlayerController>();
        initialSens = pcs.mouseSensitivity;
        audioSource = GetComponent<AudioSource>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (!isOverlapDoor)
        {
            HandleInput();
        }
    }

    void LateUpdate()
    {
        if (target == null || targetRenderer == null || playerCamera == null) return;

        try
        {
            Vector3 targetLocalPos = MoveInFrontOfObstacles();
            if (enableLerp && isLerping)
            {
                float t = (Time.time - lerpStart) / lerpTime;
                if (t >= 1f) isLerping = false;
                t = Mathf.Clamp01(t);
                target.localPosition = Vector3.Lerp(target.localPosition, targetLocalPos, t);
            }
            else
            {
                target.localPosition = targetLocalPos;
            }

            float currentDist = (transform.position - target.position).magnitude;
            if (originalDistance <= 0.01f || float.IsNaN(currentDist) || float.IsInfinity(currentDist)) return;

            float newScale = currentDist / originalDistance;
            target.localScale = originalSize * newScale;

            Vector3 newWorldPos = playerCamera.ViewportToWorldPoint(new Vector3(
                originalViewportPos.x, originalViewportPos.y,
                currentDist));

            if (!float.IsNaN(newWorldPos.x) && !float.IsInfinity(newWorldPos.x))
                target.position = newWorldPos;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LateUpdate 오류: " + ex.Message);
        }
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (target == null)
            {
                RaycastHit hit;
                if (!Physics.Raycast(transform.position, transform.forward, out hit, maxScaleDistance, targetMask)) return;

                if (audioSource && pickupSoundClip) audioSource.PlayOneShot(pickupSoundClip);

                target = hit.transform;
                targetRenderer = target.GetComponent<Renderer>();
                if (targetRenderer == null)
                {
                    Debug.LogError("Renderer가 할당되지 않았습니다. 대상 오브젝트에 Renderer 컴포넌트가 없을 수 있습니다.");
                    target = null;
                    return;
                }
                originalSize = target.localScale;
                originalDistance = Vector3.Distance(transform.position, target.position);
                if (originalDistance < 0.01f)
                {
                    Debug.LogError("픽업한 대상이 너무 가까워 Viewport 계산이 불안정합니다.");
                    target = null;
                    return;
                }

                if (playerCamera == null)
                {
                    Debug.LogError("Main Camera를 찾을 수 없습니다.");
                    target = null;
                    return;
                }

                originalViewportPos = playerCamera.WorldToViewportPoint(target.position);
                originalYRotation = target.localEulerAngles.y - transform.localEulerAngles.y;

                Rigidbody rb = target.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                Collider targetCol = target.GetComponent<Collider>();
                if (targetCol != null)
                {
                    Physics.IgnoreCollision(targetCol, player.GetComponent<Collider>(), true);
                    Physics.IgnoreCollision(targetCol, button.GetComponent<Collider>(), true);
                }

                originalLayer = target.gameObject.layer;
                target.gameObject.layer = enabledLayer;

                isLerping = enableLerp;
                lerpStart = Time.time;
                isPick = true;

                SetupGrid();
            }
            else
            {
                if (audioSource && dropSoundClip) audioSource.PlayOneShot(dropSoundClip);
                Rigidbody rb = target.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;

                Collider targetCol = target.GetComponent<Collider>();
                if (targetCol != null)
                {
                    Physics.IgnoreCollision(targetCol, player.GetComponent<Collider>(), false);
                    Physics.IgnoreCollision(targetCol, button.GetComponent<Collider>(), false);
                }

                target.gameObject.layer = originalLayer;
                target = null;
                isPick = false;
            }
        }

        if (Input.GetMouseButton(1) && target != null)
        {
            pcs.mouseSensitivity = 0;
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * initialSens * 10f;
            target.localEulerAngles = new Vector3(target.localEulerAngles.x,
                                                  target.localEulerAngles.y - mouseX,
                                                  target.localEulerAngles.z);
        }
        else if (target != null)
        {
            pcs.mouseSensitivity = initialSens;
        }

        if (Input.GetMouseButtonUp(1) && target != null)
        {
            originalYRotation = target.localEulerAngles.y - transform.localEulerAngles.y;
        }
    }

    private void SetupGrid()
    {
        shapedGrid.Clear();
        if (playerCamera == null || targetRenderer == null) return;

        for (int i = 0; i < gridRows; i++)
        {
            for (int j = 0; j < gridCols; j++)
            {
                float u = (float)i / (gridRows - 1);
                float v = (float)j / (gridCols - 1);
                Vector3 viewportPoint = new Vector3(u, v, originalDistance);

                Vector3 worldPoint = playerCamera.ViewportToWorldPoint(viewportPoint);
                Vector3 dir = worldPoint - playerCamera.transform.position;

                if (dir == Vector3.zero) continue;

                if (Physics.Raycast(playerCamera.transform.position, dir.normalized, out RaycastHit hit, maxScaleDistance, ~targetMask))
                {
                    if (hit.collider != null)
                    {
                        Vector3 localPoint = playerCamera.transform.InverseTransformPoint(hit.point);
                        shapedGrid.Add(localPoint);
                    }
                }
            }
        }
    }

    private Vector3 MoveInFrontOfObstacles()
    {
        float closestZ = float.MaxValue;
        foreach (var point in shapedGrid)
        {
            Vector3 world = playerCamera.transform.TransformPoint(point);
            Vector3 dir = world - playerCamera.transform.position;
            if (dir == Vector3.zero) continue;

            if (Physics.Raycast(playerCamera.transform.position, dir, out RaycastHit hit, maxScaleDistance, collisionMask))
            {
                Vector3 local = playerCamera.transform.InverseTransformPoint(hit.point);
                if (local.z < closestZ) closestZ = local.z;
            }
        }

        if (closestZ == float.MaxValue) closestZ = originalDistance;

        float boundsExtent = targetRenderer.bounds.extents.magnitude;
        Vector3 localPos = target.localPosition;
        localPos.z = closestZ - boundsExtent;
        return localPos;
    }
}
