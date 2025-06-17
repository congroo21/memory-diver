using UnityEngine;
using UnityEngine.SceneManagement; // 씬을 전환(게임오버 등)하기 위해 반드시 필요합니다.

public class PlayerController : MonoBehaviour
{
    // --- 이동 및 조준 관련 변수 ---
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Camera cam;
    private Vector2 mousePos;

    // --- 발사 관련 변수 (Quest 9에서 추가) ---
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 5f; // 초당 발사 횟수
    private float nextFireTime = 0f;

    // --- 체력 관련 변수 (Quest 10에서 추가) ---
    public int maxHealth = 100;     // 최대 체력
    private int currentHealth;      // 현재 체력


    // --- Unity 핵심 로직 메소드 ---

    void Start()
    {
        // 게임 시작 시 필요한 값들을 미리 찾아둡니다.
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        // 게임 시작 시 현재 체력을 최대 체력으로 설정합니다.
        currentHealth = maxHealth;
    }

    void Update()
    {
        // 매 프레임 입력 감지
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // 연사 가능한 발사 로직
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void FixedUpdate()
    {
        // 물리 엔진 업데이트에 맞춰 이동 및 회전 처리
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }


    // --- 직접 만든 기능 메소드 ---

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(firePoint.up * bulletSpeed, ForceMode2D.Impulse);
    }

    // 아이템 획득 로직 (Quest 9에서 추가)
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

    // 파워업 효과 적용 로직 (Quest 9에서 추가)
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

    // 물리적 충돌 감지 메소드 (Quest 10에서 추가)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20); // 적과 부딪히면 20의 피해를 입습니다.
            Destroy(collision.gameObject); // 부딪힌 적은 즉시 파괴됩니다.
        }
    }

    // 피해 처리 메소드 (Quest 10에서 추가)
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // 피해량만큼 현재 체력을 감소시킵니다.
        Debug.Log("Player health: " + currentHealth); // Console 창에 현재 체력을 기록합니다.

        if (currentHealth <= 0)
        {
            Debug.Log("Player Died! GAME OVER.");
            // Build Settings의 1번 씬(GameOver)을 로드합니다.
            SceneManager.LoadScene(1); 
        }
    }
}