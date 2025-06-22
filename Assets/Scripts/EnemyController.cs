// Assets/Scripts/EnemyController.cs
using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    // 기존 변수들
    public float health = 10f;
    public float speed = 3f;
    public EnemyType enemyType;
    public float dropChance = 0.2f; // 아이템 드랍 확률
    public GameObject powerUpPrefab; // 드랍할 파워업 프리팹
    private Transform player;
    private Rigidbody2D rb;

    // ★ 새로 추가된 변수
    private EnemySpawner spawner; // 자신을 생성한 스포너의 참조

    public enum EnemyType { Normal, Glitch }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (enemyType == EnemyType.Glitch)
        {
            StartCoroutine(GlitchMoveRoutine());
        }
    }

    void FixedUpdate()
    {
        if (player != null && enemyType == EnemyType.Normal)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 newPos = rb.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
    }

    IEnumerator GlitchMoveRoutine()
    {
        while (true)
        {
            // 플레이어 방향으로 돌진
            if (player != null)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.linearVelocity = direction * speed * 2f; // 글리치는 더 빠른 속도로 돌진
            }

            yield return new WaitForSeconds(0.5f); // 0.5초간 돌진

            // 잠시 정지
            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(1f); // 1초간 대기
        }
    }

    // ★ 새로 추가된 메소드: 자신을 생성한 Spawner를 등록
    public void SetSpawner(EnemySpawner owner)
    {
        this.spawner = owner;
    }
    
    // 데미지를 받는 로직 (BulletController에서 호출)
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // 적이 파괴될 때 호출되는 메소드
    private void Die()
    {
        // ★★★ 중요: 죽음을 Spawner에게 알림
        if (spawner != null)
        {
            spawner.OnEnemyDefeated(this.gameObject);
        }

        // 아이템 드랍 로직
        if (Random.value < dropChance)
        {
            if (powerUpPrefab != null)
            {
                Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            }
        }

        // 자기 자신을 파괴
        Destroy(gameObject);
    }

    // 플레이어와 충돌했을 때 처리 (기존과 동일)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어에게 데미지를 주는 로직 (PlayerController에 TakeDamage가 있다면)
            // collision.gameObject.GetComponent<PlayerController>().TakeDamage(1);
            Die(); // 플레이어와 부딪히면 자폭
        }
    }
}