using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수!

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {
        // Build Settings의 0번 씬(MainGame)을 다시 로드합니다.
        SceneManager.LoadScene(0); 
    }
}