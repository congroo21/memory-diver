using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // --- 이동 및 조준 관련 변수 ---
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Camera cam;
    private Vector2 mousePos;

    // --- 발사 관련 변수 ---
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 5f; // 초당 발사 횟수
    private float nextFireTime = 0f;

    // --- 체력 관련 변수 ---
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        currentHealth = maxHealth;

        // Collider 상태 확인 로그
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("[오류] Player에 Collider2D가 없습니다!");
        }
        else
        {
            Debug.Log($"[확인] Player Collider: {col.GetType().Name}, isTrigger: {col.isTrigger}");
        }
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * moveSpeed;

        // 이동 디버깅 로그
        Debug.Log($"[이동] velocity: {rb.linearVelocity}, position: {rb.position}");

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(firePoint.up * bulletSpeed, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerUp"))
        {
            PowerUpController powerUp = other.GetComponent<PowerUpController>();
            if (powerUp != null)
            {
                ApplyPowerUp(powerUp.type);
            }
            Destroy(other.gameObject);
        }
    }

    void ApplyPowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.FireRateIncrease:
                fireRate *= 1.2f;
                Debug.Log("Fire Rate Increased! New rate: " + fireRate);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[충돌 감지] Player가 {collision.gameObject.name} 과 충돌함.");

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20);
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player Died! GAME OVER.");
            SceneManager.LoadScene(1);
        }
    }
}
