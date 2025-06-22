// Assets/Scripts/BossController.cs
using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    // --- 보스 기본 스탯 ---
    public float maxHealth = 200f;
    public float currentHealth;

    // --- 공격 패턴 관련 ---
    public GameObject bossBulletPrefab;
    public Transform firePoint; // 총알 발사 위치
    public float attackInterval = 3f; // 다음 공격까지의 대기 시간

    // --- 참조 변수 ---
    private Transform player;
    private EnemySpawner spawner; // 이 보스를 생성한 스포너

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 보스전 시작! 공격 패턴을 반복 실행합니다.
        StartCoroutine(BattleRoutine());
    }

    // ★★★ 핵심: 여러 공격 패턴을 순차적 또는 무작위로 실행하는 메인 루틴
    IEnumerator BattleRoutine()
    {
        // 보스가 살아있는 동안 계속 반복
        while (currentHealth > 0)
        {
            // 다음 공격까지 대기
            yield return new WaitForSeconds(attackInterval);
            
            // 3가지 패턴 중 하나를 무작위로 선택하여 실행
            int patternIndex = Random.Range(0, 3);
            switch (patternIndex)
            {
                case 0:
                    yield return StartCoroutine(Pattern_BurstFire());
                    break;
                case 1:
                    yield return StartCoroutine(Pattern_CircleShot());
                    break;
                case 2:
                    yield return StartCoroutine(Pattern_Dash());
                    break;
            }
        }
    }

    // --- 공격 패턴 1: 플레이어를 향해 5연속 총알 발사 ---
    IEnumerator Pattern_BurstFire()
    {
        Debug.Log("보스 패턴: 5연발 발사!");
        int bulletCount = 5;
        float burstInterval = 0.2f;

        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bullet = Instantiate(bossBulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<BossBullet>().SetDirection((player.position - firePoint.position).normalized);
            yield return new WaitForSeconds(burstInterval);
        }
    }

    // --- 공격 패턴 2: 360도 전 방향으로 총알 발사 ---
    IEnumerator Pattern_CircleShot()
    {
        Debug.Log("보스 패턴: 원형 탄막!");
        int bulletsInCircle = 16;
        float angleStep = 360f / bulletsInCircle;

        for (int i = 0; i < bulletsInCircle; i++)
        {
            float angle = i * angleStep;
            // 각도를 2D 방향 벡터로 변환
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            
            GameObject bullet = Instantiate(bossBulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<BossBullet>().SetDirection(direction);
        }
        yield return null; // 한 프레임 대기
    }

    // --- 공격 패턴 3: 플레이어를 향해 빠르게 돌진 ---
    IEnumerator Pattern_Dash()
    {
        Debug.Log("보스 패턴: 돌진!");
        float dashSpeed = 20f;
        float dashDuration = 0.5f;
        Vector2 dashDirection = (player.position - transform.position).normalized;

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            transform.position += (Vector3)dashDirection * dashSpeed * Time.deltaTime;
            yield return null;
        }
    }
    
    // --- 공통 로직 ---
    public void SetSpawner(EnemySpawner owner)
    {
        this.spawner = owner;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // ★ 중요: 자신이 죽었음을 스포너에게 알림
        // 이 신호는 결국 DungeonManager에게 '스테이지 클리어'를 알리는 연쇄 반응을 일으킵니다.
        if (spawner != null)
        {
            spawner.OnEnemyDefeated(this.gameObject);
        }

        // 보스 사망 연출 (폭발 이펙트 등)
        
        Destroy(gameObject);
    }
}