// Assets/Scripts/EnemySpawner.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 2f;
    public int maxEnemiesInRoom = 7;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int totalSpawnedCount = 0;
    private Transform player;
    private bool isSpawning = false; 

    public UnityEvent onAllEnemiesDefeated;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void StartSpawning()
    {
        if (isSpawning)
        {
            return;
        }
        isSpawning = true;

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
        Vector3 spawnPosition = GetRandomSpawnPosition();
        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyInstance = Instantiate(enemyPrefabs[index], spawnPosition, Quaternion.identity);

        // EnemyController가 있는 경우에만 SetSpawner를 호출하도록 안전장치 추가
        EnemyController enemyController = enemyInstance.GetComponent<EnemyController>();
        if(enemyController != null)
        {
            enemyController.SetSpawner(this);
        }

        // BossController가 있는 경우
        BossController bossController = enemyInstance.GetComponent<BossController>();
        if(bossController != null)
        {
            bossController.SetSpawner(this);
        }

        spawnedEnemies.Add(enemyInstance);
    }
    
    public void OnEnemyDefeated(GameObject defeatedEnemy)
    {
        if (spawnedEnemies.Contains(defeatedEnemy))
        {
            spawnedEnemies.Remove(defeatedEnemy);
        }

        if (totalSpawnedCount >= maxEnemiesInRoom && spawnedEnemies.Count == 0)
        {
            Debug.Log("이 스포너의 모든 적이 처치되었습니다.");
            if (onAllEnemiesDefeated != null)
            {
                onAllEnemiesDefeated.Invoke();
            }
            
            isSpawning = false;
            this.enabled = false; 
        }
    }

    // ★★★ 여기가 수정된 핵심 부분입니다 ★★★
    Vector3 GetRandomSpawnPosition()
    {
        // 스포너를 기준으로 한 상대적인 랜덤 오프셋 값을 계산
        float spawnX = Random.Range(-10f, 10f); // 값은 방 크기에 맞게 조절
        float spawnY = Random.Range(-5f, 5f);  // 값은 방 크기에 맞게 조절
        Vector3 randomOffset = new Vector3(spawnX, spawnY, 0);

        // 스포너 자신의 위치에 랜덤 오프셋을 더하여 최종 스폰 위치를 결정
        return transform.position + randomOffset;
    }
}