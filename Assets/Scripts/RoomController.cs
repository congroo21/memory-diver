// Assets/Scripts/RoomController.cs
using UnityEngine;
using UnityEngine.Events;

public class RoomController : MonoBehaviour
{
    public bool isCleared = false;
    public DoorController[] doors;
    public EnemySpawner enemySpawner; // 각 방은 자신의 EnemySpawner를 가짐

    void Start()
    {
        // 시작 시에는 문을 잠그고, 적 스포너를 비활성화 상태로 둔다.
        if (enemySpawner != null)
        {
            enemySpawner.enabled = false;
            // EnemySpawner가 모든 적을 처치했을 때 OnRoomClear 메소드를 호출하도록 연결
            enemySpawner.onAllEnemiesDefeated.AddListener(OnRoomClear);
        }
    }

    // 플레이어가 이 방에 들어왔을 때 DungeonManager가 호출
    public void OnPlayerEnter()
    {
        Debug.Log(gameObject.name + " 방에 진입했습니다.");
        if (isCleared)
        {
            // 이미 클리어된 방이라면 문을 바로 열어줌
            UnlockDoors();
        }
        else
        {
            // 클리어되지 않은 방이라면
            LockDoors();
            // 적 스포너가 있다면 활성화하여 전투 시작
            if (enemySpawner != null)
            {
                enemySpawner.enabled = true;
                enemySpawner.StartSpawning();
            }
            else // 적이 없는 방은 즉시 클리어 처리
            {
                OnRoomClear();
            }
        }
    }

    // 모든 적이 처치되었을 때 EnemySpawner가 호출
    public void OnRoomClear()
    {
        Debug.Log(gameObject.name + " 방 클리어!");
        isCleared = true;
        UnlockDoors();

        // 이 방이 보스룸이라면 DungeonManager에 알림
        if (this == DungeonManager.instance.bossRoom)
        {
            DungeonManager.instance.OnBossRoomCleared();
        }
    }

    public void LockDoors()
    {
        foreach (var door in doors)
        {
            door.Lock();
        }
    }

    public void UnlockDoors()
    {
        foreach (var door in doors)
        {
            door.Unlock();
        }
    }
}