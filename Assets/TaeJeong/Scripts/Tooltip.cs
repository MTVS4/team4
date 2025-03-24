using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public GameObject cursorDefault;
    public GameObject cursorHand;
    public GameObject cursorGrabbed;
    public LayerMask groundMask;      // 지면이 속한 레이어
    private PickResize pickResize;
    private bool haveOutline = false;
    private GameObject target;

    void Start()
    {
        pickResize = GetComponent<PickResize>();
    }

    void Update()
    {
        if (pickResize.target != null)
        {
            // PickResize에서 대상이 이미 잡혔으면 grabbed 커서를 표시합니다.
            target = null;
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
            SetCursor(0, 0, 1);
        }
        else
        {
            // 두 Raycast를 수행합니다.
            // 1. 지면(Ray)을 위한 Raycast: groundMask를 사용합니다.
            RaycastHit hitGround;
            bool groundHit = Physics.Raycast(transform.position, transform.forward, out hitGround, pickResize.maxScaleDistance, groundMask);

            // 2. 대상(Ray)을 위한 Raycast: pickResize.targetMask를 사용합니다.
            RaycastHit hitTarget;
            bool targetHit = Physics.Raycast(transform.position, transform.forward, out hitTarget, pickResize.maxScaleDistance, pickResize.targetMask);

            // 대상이 hit되었다면,
            if (targetHit)
            {
                // 만약 지면이 hit되었고, 그 거리가 대상보다 짧다면 대상은 무시합니다.
                if (groundHit && hitGround.distance < hitTarget.distance)
                {
                    ClearTooltip();
                }
                else
                {
                    // 유효한 대상이 있으므로 tooltip 업데이트
                    if (target == null)
                    {
                        target = hitTarget.transform.gameObject;
                        var sp = target.GetComponent<SuperProp>();
                        if (sp != null)
                        {
                            var outline = target.GetComponent<Outline>();
                            haveOutline = sp.outline && outline != null;
                            if (haveOutline)
                            {
                                outline.OutlineColor = sp.pointedColor;
                                outline.enabled = true;
                            }
                        }
                    }
                    SetCursor(0, 1, 0);
                }
            }
            else
            {
                ClearTooltip();
            }
        }
    }

    // 대상이 없거나, 지면에 가려진 경우 tooltip을 초기화합니다.
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

    // 커서 UI의 CanvasGroup alpha 값을 한 번에 설정합니다.
    void SetCursor(float defaultAlpha, float handAlpha, float grabbedAlpha)
    {
        cursorDefault.GetComponent<CanvasGroup>().alpha = defaultAlpha;
        cursorHand.GetComponent<CanvasGroup>().alpha = handAlpha;
        cursorGrabbed.GetComponent<CanvasGroup>().alpha = grabbedAlpha;
    }
}
