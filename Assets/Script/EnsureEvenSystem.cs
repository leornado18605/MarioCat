using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EnsureEventSystem : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Ensure();
        SceneManager.sceneLoaded += (_, __) => Ensure();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= (_, __) => Ensure();
    }

    private void Ensure()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;

        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
        DontDestroyOnLoad(go);
    }
}