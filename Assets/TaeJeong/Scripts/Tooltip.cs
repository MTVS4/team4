using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public GameObject cursorDefault;
    public GameObject cursorHand;
    public GameObject cursorGrabbed;
    private PickResize pickResize;
    private bool haveOutline = false;
    private GameObject target;
    void Start() {
        pickResize = GetComponent<PickResize>();
    }

    void Update() {
        if (pickResize.target != null) {
            target = null;
            SuperProp sp = pickResize.target.GetComponent<SuperProp>();
            if (sp != null) {
                if (sp.outline) {
                    Outline OutLine = pickResize.target.GetComponent<Outline>();
                    OutLine.OutlineColor = sp.holdColor;
                    OutLine.enabled = true;
                }
            }
            cursorDefault.GetComponent<CanvasGroup>().alpha = 0;
            cursorHand.GetComponent<CanvasGroup>().alpha = 0;
            cursorGrabbed.GetComponent<CanvasGroup>().alpha = 1;
        } else {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, pickResize.maxScaleDistance, pickResize.targetMask)) {
                if (target == null) {
                    target = hit.transform.gameObject;
                    SuperProp sp = target.GetComponent<SuperProp>();
                    if (sp != null) {
                        Outline OutLine = target.GetComponent<Outline>();
                        haveOutline = sp.outline && OutLine != null;
                        if (haveOutline) {
                            OutLine.OutlineColor = sp.pointedColor;
                            OutLine.enabled = true;
                        }
                        cursorDefault.GetComponent<CanvasGroup>().alpha = 0;
                        cursorHand.GetComponent<CanvasGroup>().alpha = 1;
                        cursorGrabbed.GetComponent<CanvasGroup>().alpha = 0;
                    }
                }
            } else {
                if (target != null && haveOutline) {
                    target.GetComponent<Outline>().enabled = false;
                    haveOutline = false;
                }
                cursorDefault.GetComponent<CanvasGroup>().alpha = 1;
                cursorHand.GetComponent<CanvasGroup>().alpha = 0;
                cursorGrabbed.GetComponent<CanvasGroup>().alpha = 0;
                target = null;
            }
        }
    }
}
