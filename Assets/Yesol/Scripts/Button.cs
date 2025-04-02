using System;
using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour
{
    public GameObject door;  
    public GameObject exit;
    private float green;
    private float red = 3f;
    private Renderer renderer;
    private Material material;
    
    public float slideDistance = 5f; 
    public float slideSpeed = 2f;

    private bool isOpened = false;  
    private Vector3 closedPosition;  
    private Vector3 openedPosition;  
    private Coroutine moveCoroutine;
    
    public AudioSource source;
    public AudioClip exitSfx;
    bool playedSound = false;

    void Awake()
    {
        renderer = exit.GetComponent<Renderer>();
        material = renderer.materials[1];
    }
    
    void Start()
    {
        closedPosition = door.transform.position;
        
        if (door.transform.eulerAngles.y < 180 && door.transform.eulerAngles.y > 0)
        {
            openedPosition = closedPosition - new Vector3(0, 0, slideDistance);
        }
        else
        {
            openedPosition = closedPosition - new Vector3(slideDistance, 0, 0);
        }
        
    }

    private void Update()
    {
        material.SetFloat("_GreenEmissive" , green);
        material.SetFloat("_RedEmissive" , red);
    }

    void OnTriggerStay(Collider col)
    {
        if (!isOpened) 
        {
            isOpened = true; 
            StartMovingDoor(openedPosition);
            
            red = 0f;
            green = 3f;
        }

        if (!playedSound)
        {
          ExitSound();
          playedSound = true;
        }
        
    }

    void OnTriggerExit(Collider col)
    {
        if (isOpened)
        {
            isOpened = false;
            StartMovingDoor(closedPosition);
            
            red = 3f;
            green = 0f;
            
            playedSound = false;
        }
    }

    void StartMovingDoor(Vector3 targetPosition)
    {
        if (moveCoroutine != null) 
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveDoor(targetPosition));
    }

    IEnumerator MoveDoor(Vector3 targetPosition)
    { 
        while (Vector3.Distance(door.transform.position, targetPosition) > 0) 
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
        door.transform.position = targetPosition;   
    }

    public void ExitSound()
    {
        source.PlayOneShot(exitSfx);
    }
}


