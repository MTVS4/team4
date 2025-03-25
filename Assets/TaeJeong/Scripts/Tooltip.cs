using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tooltip 스크립트는 플레이어 시점에서 Raycast를 사용하여 픽업 대상(또는 Hover 대상)을 감지하고,
/// 해당 대상에 따라 커서(UI)의 모양과 대상의 Outline 효과를 제어합니다.
/// 지면(Ground) 레이어가 먼저 감지되면 대상이 가려진 것으로 간주하여 기본 커서를 유지합니다.
/// </summary>
public class Tooltip : MonoBehaviour
{
    // 커서 UI 오브젝트 참조 (각각 CanvasGroup 컴포넌트를 가지고 있음)
    public GameObject cursorDefault;
    public GameObject cursorHand;
    public GameObject cursorGrabbed;
    
    // 지면(ground) 레이어 마스크
    public LayerMask groundMask;

    // PickResize 스크립트 참조 (픽업 가능한 대상 제어)
    private PickResize pickResize;
    
    // 커서 변경 시 Outline 효과 적용 여부 플래그
    private bool haveOutline = false;
    
    // 현재 Hover 중인 대상 (픽업 대상이 아닐 때)
    private GameObject target;

    // 캐싱한 CanvasGroup 컴포넌트 (GetComponent 호출을 줄임)
    private CanvasGroup defaultCanvasGroup;
    private CanvasGroup handCanvasGroup;
    private CanvasGroup grabbedCanvasGroup;

    void Start()
    {
        // PickResize 컴포넌트 참조 획득
        pickResize = GetComponent<PickResize>();
        // 커서 오브젝트의 CanvasGroup을 캐싱
        if (cursorDefault != null)
            defaultCanvasGroup = cursorDefault.GetComponent<CanvasGroup>();
        if (cursorHand != null)
            handCanvasGroup = cursorHand.GetComponent<CanvasGroup>();
        if (cursorGrabbed != null)
            grabbedCanvasGroup = cursorGrabbed.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // 픽업된 대상이 있는 경우 (PickResize.target이 할당된 경우)
        if (pickResize.target != null)
        {
            // Hover 대상은 해제
            target = null;
            // 픽업된 대상의 SuperProp 스크립트를 확인하여 Outline 효과를 적용
            var sp = pickResize.target.GetComponent<SuperProp>();
            if (sp != null)
            {
                var outline = pickResize.target.GetComponent<Outline>();
                if (sp.outline && outline != null)
                {
                    outline.OutlineColor = sp.holdColor;
                    outline.enabled = true;
                }
            }
            // grabbed 커서를 표시 (default, hand 커서는 숨김)
            SetCursor(0, 0, 1);
        }
        else
        {
            // 픽업 대상이 없는 경우, Raycast로 Hover 대상 탐색
            
            // Raycast로 지면(Ground)와 충돌 여부를 확인
            RaycastHit hitGround;
            bool groundHit = Physics.Raycast(transform.position, transform.forward, out hitGround, pickResize.maxScaleDistance, groundMask);
            
            // Raycast로 픽업 대상과 충돌 여부를 확인 (PickResize.targetMask 사용)
            RaycastHit hitTarget;
            bool targetHit = Physics.Raycast(transform.position, transform.forward, out hitTarget, pickResize.maxScaleDistance, pickResize.targetMask);
            
            if (targetHit)
            {
                // 만약 지면이 대상보다 먼저 충돌하면 대상이 가려진 것으로 간주
                if (groundHit && hitGround.distance < hitTarget.distance)
                {
                    ClearTooltip();
                }
                else
                {
                    // 유효한 대상이 감지되었을 때 처음 한 번만 대상 할당 및 Outline 효과 적용
                    if (target == null)
                    {
                        target = hitTarget.transform.gameObject;
                        var sp = target.GetComponent<SuperProp>();
                        if (sp != null)
                        {
                            var outline = target.GetComponent<Outline>();
                            // SuperProp 설정에 따라 Outline 적용 여부 결정
                            haveOutline = sp.outline && outline != null;
                            if (haveOutline)
                            {
                                outline.OutlineColor = sp.pointedColor;
                                outline.enabled = true;
                            }
                        }
                    }
                    // hand 커서를 표시 (default, grabbed 커서는 숨김)
                    SetCursor(0, 1, 0);
                }
            }
            else
            {
                // 대상이 감지되지 않으면 Tooltip 초기화
                ClearTooltip();
            }
        }
    }

    /// <summary>
    /// ClearTooltip: 현재 Hover 대상과 관련된 Outline 효과를 해제하고 기본 커서를 표시합니다.
    /// </summary>
    void ClearTooltip()
    {
        if (target != null && haveOutline)
        {
            var outline = target.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
            haveOutline = false;
        }
        target = null;
        SetCursor(1, 0, 0);
    }
    
    /// <summary>
    /// SetCursor: 각 커서 UI(CanvasGroup)의 알파 값을 설정하여 커서 모양을 전환합니다.
    /// </summary>
    /// <param name="defaultAlpha">기본 커서의 알파 값</param>
    /// <param name="handAlpha">손 모양 커서의 알파 값</param>
    /// <param name="grabbedAlpha">잡힌 커서의 알파 값</param>
    void SetCursor(float defaultAlpha, float handAlpha, float grabbedAlpha)
    {
        if (defaultCanvasGroup != null)
            defaultCanvasGroup.alpha = defaultAlpha;
        if (handCanvasGroup != null)
            handCanvasGroup.alpha = handAlpha;
        if (grabbedCanvasGroup != null)
            grabbedCanvasGroup.alpha = grabbedAlpha;
    }
}
