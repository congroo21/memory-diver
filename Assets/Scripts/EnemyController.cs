using UnityEngine;
using System.Collections; // 코루틴(IEnumerator)을 사용하기 위해 반드시 필요합니다.

// 적의 유형을 정의합니다. 앞으로 더 많은 엘리트 타입을 여기에 추가할 수 있습니다.
public enum EnemyType { Normal, Glitch };

public class EnemyController : MonoBehaviour
{
    [Header("AI Type")] // Inspector 창에서 보기 좋게 섹션을 나눕니다.
    public EnemyType enemyType = EnemyType.Normal; // 이 적의 유형을 정하는 스위치

    [Header("Common Stats")]
    public float speed = 3f;
    public int health = 30;

    [Header("Item Drop")]
    public GameObject powerUpPrefab;
    [Range(0f, 1f)] public float dropChance = 0.1f;

    [Header("Glitch-Specific Stats")]
    public float burstSpeed = 10f;    // 글리치가 돌진할 때의 속도
    public float burstDuration = 0.5f;  // 돌진을 유지하는 시간
    public float pauseDuration = 1f;    // 돌진 후 멈춰있는 시간

    // 비공개(private) 변수들
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        // 공통적으로 필요한 컴포넌트 및 오브젝트를 미리 찾아둡니다.
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // 만약 이 적의 타입이 '글리치'라면, 특별한 움직임 로직을 시작시킵니다.
        if (enemyType == EnemyType.Glitch)
        {
            StartCoroutine(GlitchMovementRoutine());
        }
    }

    void Update()
    {
        // '일반' 타입의 적만 이 단순 추적 로직을 매 프레임 실행합니다.
        if (enemyType == EnemyType.Normal && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    // 외부(총알 등)에서 호출하는 '피격' 처리 메소드입니다.
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // 적의 '죽음'을 처리하는 내부 메소드입니다.
    void Die()
    {
        Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // '글리치' 타입 전용 움직임 로직입니다. (코루틴)
    IEnumerator GlitchMovementRoutine()
    {
        // 이 로직은 적이 살아있는 동안 무한히 반복됩니다.
        while (true)
        {
            if (player != null)
            {
                // 1. 돌진
                Vector2 direction = (player.position - transform.position).normalized;
                rb.AddForce(direction * burstSpeed, ForceMode2D.Impulse);
                
                // 2. 돌진 유지 시간만큼 대기
                yield return new WaitForSeconds(burstDuration);
                
                // 3. 정지
                rb.linearVelocity = Vector2.zero;

                // 4. 정지 상태로 대기
                yield return new WaitForSeconds(pauseDuration);
            }
            else // 혹시 플레이어가 없다면, 그냥 1초 대기
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}