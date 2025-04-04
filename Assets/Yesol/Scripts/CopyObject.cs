using System.Collections;
using UnityEngine;

public class CopyObject : MonoBehaviour
{
    public GameObject originObject;
    public float scale = 0.9f;
    public float speed;
    private bool isDestroying = false; 

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isDestroying) return;

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

                    if (copyScript.originObject == trueOrigin && !copyScript.isDestroying)
                    {
                        copy.GetComponent<Rigidbody>().isKinematic = true;
                        copyScript.StartCoroutine(copyScript.MoveDestroy());
                    }
                }
            }
        }
    }

    IEnumerator MoveDestroy()
    {
        isDestroying = true;  
        Vector3 targetPos = originObject.transform.position;

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, originObject.transform.localScale, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        Destroy(gameObject);
    }
}