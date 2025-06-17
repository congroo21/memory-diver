using UnityEngine;

public class BulletController : MonoBehaviour
{
    // --- 총알의 기본 속성 ---
    // 총알이 적에게 입힐 피해량입니다. (Quest 10에서 추가)
    public int damage = 10;


    // --- Unity 핵심 로직 메소드 ---

    void Start()
    {
        // 총알이 생성된 후 3초가 지나면 자동으로 파괴됩니다.
        // 화면에 총알이 무한히 쌓이는 것을 방지합니다.
        Destroy(gameObject, 3f);
    }

    // 다른 Collider와 부딪혔을 때 (Is Trigger가 켜져 있을 때) 호출됩니다.
    void OnTriggerEnter2D(Collider2D other)
    {
        // 부딪힌 대상의 태그가 "Enemy"가 맞는지 확인합니다.
        if (other.CompareTag("Enemy"))
        {
            // 부딪힌 적의 EnemyController 스크립트를 가져옵니다.
            EnemyController enemy = other.GetComponent<EnemyController>();
            
            // 만약 스크립트를 성공적으로 가져왔다면,
            if (enemy != null)
            {
                // 적의 TakeDamage 메소드를 호출하여 피해를 입힙니다. (Quest 10에서 변경된 핵심 로직)
                enemy.TakeDamage(damage);
            }
            
            // 적과 부딪혔으므로, 총알 자신도 파괴됩니다.
            Destroy(gameObject);      
        }
    }
}