using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class PlayerPersist : MonoBehaviour
{
    private static PlayerPersist instance;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SnapToSpawnA());
    }

    private IEnumerator SnapToSpawnA()
    {
        yield return null;

        var spawn = GameObject.FindGameObjectWithTag("SpawnLevel2");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;

            var rb                    = GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = Vector2.zero;
        }

        var health = GetComponent<PlayerHealth>();
        if (health != null)
        {
            var ui = Object.FindFirstObjectByType<HealthUI>();
            if (ui) ui.SetHearts(health.CurrentHp);
        }

        var vcam              = Object.FindFirstObjectByType<CinemachineCamera>();
        if (vcam) vcam.Follow = transform;
    }

}