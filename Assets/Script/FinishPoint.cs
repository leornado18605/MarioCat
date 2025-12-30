using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float countdownSeconds = 3f;
    [SerializeField] private int level1BuildIndex = 2;
    [SerializeField] private int level2BuildIndex = 3;
    [SerializeField] private string selectCharacterSceneName = "SelectCharacter";

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        Time.timeScale = 1f;

        // Nếu có UI persist thì dùng countdown + panel
        if (UIPersist.I != null)
        {
            UIPersist.I.ShowCountdown(countdownSeconds, DoSceneChange);
        }
        else
        {
            // fallback: không có UI thì chuyển scene luôn
            DoSceneChange();
        }
    }

    private void DoSceneChange()
    {
        int current = SceneManager.GetActiveScene().buildIndex;

        if (current == level1BuildIndex)
        {
            LoadScene(level2BuildIndex);
            return;
        }

        if (current == level2BuildIndex)
        {
            var playerPersist = Object.FindFirstObjectByType<PlayerPersist>();
            if (playerPersist) Destroy(playerPersist.gameObject);

            var uiPersist = Object.FindFirstObjectByType<UIPersist>();
            if (uiPersist) Destroy(uiPersist.gameObject);

            LoadScene(selectCharacterSceneName);
            return;
        }

        LoadScene(current + 1);
    }

    private void LoadScene(int buildIndex)
    {
        if (SceneTransition.I != null) SceneTransition.I.LoadScene(buildIndex);
        else SceneManager.LoadScene(buildIndex);
    }

    private void LoadScene(string sceneName)
    {
        if (SceneTransition.I != null) SceneTransition.I.LoadScene(sceneName);
        else SceneManager.LoadScene(sceneName);
    }
}