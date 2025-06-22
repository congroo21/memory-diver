// Assets/Scripts/DungeonManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance;

    public RoomController startingRoom;
    public RoomController currentRoom;
    public RoomController bossRoom; // 보스룸을 명시적으로 지정
    public RoomCameraController roomCamera;
    
    private PlayerController player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        // 게임 시작 시, 시작 방으로 이동
        TransitionToRoom(startingRoom, player.transform.position); 
    }

    // 새로운 방으로 플레이어와 카메라를 이동시키는 핵심 메소드
    public void TransitionToRoom(RoomController newRoom, Vector3 doorEntryPosition)
    {
        if (newRoom == null) return;

        currentRoom = newRoom;
        
        // 플레이어 위치를 문 바로 앞으로 재설정
        player.transform.position = doorEntryPosition;

        // 카메라가 새로운 방을 비추도록 명령
        roomCamera.MoveToNewRoom(newRoom.transform);

        // 새로운 방의 로직 활성화
        newRoom.OnPlayerEnter();
    }
    
    // 보스룸이 클리어되었을 때 호출될 메소드
    public void OnBossRoomCleared()
    {
        Debug.Log("스테이지 클리어! 다음 스테이지로 이동합니다.");
        // 여기에 다음 씬을 로드하는 로직을 추가할 수 있습니다.
        // 예: SceneManager.LoadScene("NextStageScene");
    }
}