using System.Collections;
using UnityEngine;

public class CopyObject : MonoBehaviour
{
    public GameObject originObject;
    public float scale = 0.9f; 
    public float speed = 2f;
    
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float objectLength = GetComponent<Renderer>().bounds.size.x;
            Vector3 spawnPoint = transform.position - Camera.main.transform.forward * objectLength;
            
            GameObject newObj = Instantiate(gameObject, spawnPoint, Quaternion.identity);
            newObj.layer = LayerMask.NameToLayer("CopyLayer");
            newObj.tag = "Copy";
            newObj.transform.localScale = transform.localScale * scale;
            
            CopyObject newScript = newObj.GetComponent<CopyObject>();
            newScript.originObject = (tag == "Copy" && originObject != null) ? originObject : this.gameObject;
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            GameObject trueOrigin = (tag == "Copy" && originObject != null) ? originObject : this.gameObject;
            GameObject[] allCopies = GameObject.FindGameObjectsWithTag("Copy");
            
            foreach (GameObject copy in allCopies)
            {
                if (copy != null)
                {
                    CopyObject copyScript = copy.GetComponent<CopyObject>();
                    
                    if (copyScript.originObject == trueOrigin)
                    {
                        copy.GetComponent<Rigidbody>().isKinematic = true;
                        StartCoroutine(MoveDestroy(copy));
                    }
                }
            }
        }
    }

    IEnumerator MoveDestroy(GameObject clone)
    {
        CopyObject cloneScript = clone.GetComponent<CopyObject>();
        Vector3 targetPos = cloneScript.originObject.transform.position;
        
        while (Vector3.Distance(clone.transform.position, targetPos) > 0.1f)
        {
            clone.transform.position = Vector3.Lerp(clone.transform.position, targetPos, speed * Time.deltaTime);
            clone.transform.localScale = Vector3.Lerp(clone.transform.localScale, cloneScript.originObject.transform.localScale, speed * Time.deltaTime);
            yield return null;
        }
        
        if (clone != null)
        {
            clone.transform.position = targetPos;
            Destroy(clone);
        }
    }
} 