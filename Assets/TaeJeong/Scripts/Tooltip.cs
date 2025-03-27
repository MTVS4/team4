using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{

    public GameObject cursorDefault;
    public GameObject cursorHand;
    public GameObject cursorGrabbed;
    
    public LayerMask groundMask;
    
    private PickResize pickResize;
    
    private bool haveOutline = false;
    
    private GameObject target;

    private CanvasGroup defaultCanvasGroup;
    private CanvasGroup handCanvasGroup;
    private CanvasGroup grabbedCanvasGroup;

    void Start()
    {
        pickResize = GetComponent<PickResize>();
        if (cursorDefault != null)
            defaultCanvasGroup = cursorDefault.GetComponent<CanvasGroup>();
        if (cursorHand != null)
            handCanvasGroup = cursorHand.GetComponent<CanvasGroup>();
        if (cursorGrabbed != null)
            grabbedCanvasGroup = cursorGrabbed.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (pickResize.target != null)
        {
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
            RaycastHit hitGround;
            bool groundHit = Physics.Raycast(transform.position, transform.forward, out hitGround, pickResize.maxScaleDistance, groundMask);
            
            RaycastHit hitTarget;
            bool targetHit = Physics.Raycast(transform.position, transform.forward, out hitTarget, pickResize.maxScaleDistance, pickResize.targetMask);
            
            if (targetHit)
            {
                if (groundHit && hitGround.distance < hitTarget.distance)
                {
                    ClearTooltip();
                }
                else
                {
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
