// Assets/Scripts/BossBullet.cs
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 2; // 보스 총알은 더 강력하게!
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    // 화면 밖으로 나가면 스스로 파괴 (메모리 관리)
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 부딪혔을 때
        if (other.CompareTag("Player"))
        {
            // 플레이어에게 데미지를 주는 로직 (Player 스크립트에 TakeDamage가 있다면)
            // other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Debug.Log("플레이어가 보스 총알에 맞음!");
            Destroy(gameObject); // 부딪히면 파괴
        }
    }
}