// Assets/Scripts/EnemySpawner.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; // UnityEvent를 사용하기 위해 추가

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 2f;
    public int maxEnemiesInRoom = 10; // 방에 스폰할 총 적의 수

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int totalSpawnedCount = 0;
    private Transform player;

    // 방 클리어 시 RoomController에 알리기 위한 이벤트
    public UnityEvent onAllEnemiesDefeated;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // RoomController가 호출할 스폰 시작 메소드
    public void StartSpawning()
    {
        totalSpawnedCount = 0;
        spawnedEnemies.Clear();
        StartCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (totalSpawnedCount < maxEnemiesInRoom)
        {
            SpawnEnemy();
            totalSpawnedCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    void SpawnEnemy()
    {
        // (기존 스폰 로직과 거의 동일)
        Vector3 spawnPosition = GetRandomSpawnPosition();
        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyInstance = Instantiate(enemyPrefabs[index], spawnPosition, Quaternion.identity);

        // ★★★ 중요: 생성된 적에게 이 스포너 정보를 넘겨줌
        enemyInstance.GetComponent<EnemyController>().SetSpawner(this);
        spawnedEnemies.Add(enemyInstance);
    }
    
    // ★★★ 새로 추가된 부분: 적이 죽었을 때 호출될 메소드
    public void OnEnemyDefeated(GameObject defeatedEnemy)
    {
        spawnedEnemies.Remove(defeatedEnemy);

        // 스폰할 적을 모두 스폰했고, 맵에 남은 적이 없다면
        if (totalSpawnedCount >= maxEnemiesInRoom && spawnedEnemies.Count == 0)
        {
            Debug.Log("이 스포너의 모든 적이 처치되었습니다.");
            if (onAllEnemiesDefeated != null)
            {
                onAllEnemiesDefeated.Invoke(); // RoomController에 방이 클리어됐음을 알림
            }
            this.enabled = false; // 스포너 비활성화
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // (Congroo님의 기존 GetRandomSpawnPosition 로직 그대로 사용)
        float spawnX = Random.Range(-10f, 10f); // 값은 방 크기에 맞게 조절
        float spawnY = Random.Range(-5f, 5f);  // 값은 방 크기에 맞게 조절
        return new Vector3(spawnX, spawnY, 0);
    }
}