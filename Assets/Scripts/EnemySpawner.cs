using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // --- 스포너 설정 변수 ---
    // 생성할 적 프리팹 '배열'. 이제 여러 종류의 적을 담을 수 있습니다.
    public GameObject[] enemyPrefabs; 
    
    [Tooltip("초당 몇 마리의 적을 생성할지 결정합니다.")]
    public float spawnRate = 2f;

    [Tooltip("화면 가장자리에서 얼마나 떨어진 곳에 생성될지 결정합니다.")]
    public float spawnAreaPadding = 1f;

    [Tooltip("새로 생성되는 적이 엘리트일 확률입니다.")]
    [Range(0f, 1f)] public float eliteSpawnChance = 0.2f; // 엘리트 스폰 확률 (20%)


    // --- 내부 동작 변수 ---
    private float nextSpawnTime;
    private Camera cam;
    private Vector2 screenBounds;

    void Start()
    {
        cam = Camera.main;
        // 화면 경계를 월드 좌표로 미리 계산해둡니다.
        screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z));
    }

    void Update()
    {
        // 정해진 시간마다 적 생성 로직을 호출합니다.
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    void SpawnEnemy()
    {
        GameObject prefabToSpawn; // 이번에 소환할 프리팹을 담을 변수

        // 1. 어떤 적을 소환할지 확률에 따라 결정합니다.
        if (Random.value < eliteSpawnChance)
        {
            // 엘리트를 소환하기로 결정 (배열의 1번 인덱스)
            // 주의: enemyPrefabs 배열에 최소 2개의 프리팹이 할당되어 있어야 합니다.
            prefabToSpawn = enemyPrefabs[1];
        }
        else
        {
            // 일반 적을 소환하기로 결정 (배열의 0번 인덱스)
            prefabToSpawn = enemyPrefabs[0];
        }

        // 2. 어디에 소환할지 화면 밖 랜덤 위치를 계산합니다.
        Vector2 spawnPosition = Vector2.zero;
        int side = Random.Range(0, 4); 

        switch (side)
        {
            case 0: spawnPosition = new Vector2(Random.Range(-screenBounds.x, screenBounds.x), screenBounds.y + spawnAreaPadding); break;
            case 1: spawnPosition = new Vector2(Random.Range(-screenBounds.x, screenBounds.x), -screenBounds.y - spawnAreaPadding); break;
            case 2: spawnPosition = new Vector2(screenBounds.x + spawnAreaPadding, Random.Range(-screenBounds.y, screenBounds.y)); break;
            case 3: spawnPosition = new Vector2(-screenBounds.x - spawnAreaPadding, Random.Range(-screenBounds.y, screenBounds.y)); break;
        }

        // 3. 결정된 프리팹을 결정된 위치에 생성(소환)합니다.
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}