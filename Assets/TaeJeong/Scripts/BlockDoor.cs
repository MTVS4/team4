using System;
using UnityEngine;

public class BlockDoor : MonoBehaviour
{

    /// 플레이어가 오브젝트를 들고 있을 때
    /// 넘어가지지 않는 문을 만든다.
    /// 들고 있지 않으면 넘어갈 수 있다.
    /// 문에 겹쳐져 있을 때는 잡기 불가능
    
    public PickResize playerPick;
    private Collider blockDoorCollider;

    void Awake()
    {
        blockDoorCollider = GetComponent<Collider>();
    }
    
    void Update()
    {
        if (playerPick.target != null)
        {
            blockDoorCollider.isTrigger = false;
        }
        else
        {
            blockDoorCollider.isTrigger = true;
        }
    }
    
    // isOverlapDoor = false 일 때 잡기 가능
    private void OnTriggerEnter(Collider other)
    {
        playerPick.isOverlapDoor = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerPick.isOverlapDoor = false;
    }
}
