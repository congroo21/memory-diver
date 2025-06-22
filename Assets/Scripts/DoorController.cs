// Assets/Scripts/DoorController.cs
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public RoomController connectedRoom; // 이 문이 연결된 다음 방
    public Transform entryPoint;         // 다음 방에 들어갈 때 플레이어의 위치
    private bool isLocked = true;
    private Collider2D doorCollider;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isLocked && other.CompareTag("Player"))
        {
            Debug.Log(connectedRoom.name + "으로 이동합니다.");
            DungeonManager.instance.TransitionToRoom(connectedRoom, entryPoint.position);
        }
    }
    
    // 문을 잠그는 메소드 (충돌 비활성화, 모습 변경 등)
    public void Lock()
    {
        isLocked = true;
        // 예: 문이 닫히는 애니메이션, 색상 변경 등
        GetComponent<SpriteRenderer>().color = Color.red; 
    }

    // 문을 여는 메소드 (충돌 활성화)
    public void Unlock()
    {
        isLocked = false;
        // 예: 문이 열리는 애니메이션, 색상 변경 등
        GetComponent<SpriteRenderer>().color = Color.green;
    }
}