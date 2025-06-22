using UnityEngine;

public class WallLogger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[벽 충돌] {collision.gameObject.name} 이(가) 벽에 닿았습니다.");
    }
}
