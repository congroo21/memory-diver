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

    // ★★★ 새로 추가된 상태 변수 ★★★
    private bool isSpawning = false; 

    public UnityEvent onAllEnemiesDefeated;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void StartSpawning()
    {
        // ★★★ 수정된 부분: 이미 스폰 중이면 아무것도 하지 않고 리턴 ★★★
        if (isSpawning)
        {
            return;
        }
        isSpawning = true; // 스폰 시작 상태로 변경

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

        enemyInstance.GetComponent<EnemyController>().SetSpawner(this);
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

            // ★★★ 수정된 부분: 방이 클리어되면 스폰 상태를 리셋 ★★★
            isSpawning = false;
            this.enabled = false; 
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float spawnX = Random.Range(-10f, 10f);
        float spawnY = Random.Range(-5f, 5f);
        return new Vector3(spawnX, spawnY, 0);
    }
}