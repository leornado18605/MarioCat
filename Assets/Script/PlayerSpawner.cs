using Unity.Cinemachine;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private GameObject[] _characterPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private Transform _spawnPoint;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineCamera _virtualCamera;

    private void Start()
    {
        if (FindFirstObjectByType<PlayerPersist>() != null)
        {
            if (_virtualCamera == null) _virtualCamera        = FindFirstObjectByType<CinemachineCamera>();
            if (_virtualCamera != null) _virtualCamera.Follow = FindFirstObjectByType<PlayerPersist>().transform;
            return;
        }

        if (_virtualCamera == null)
            _virtualCamera = FindFirstObjectByType<CinemachineCamera>();

        SpawnPlayer();
    }


    private void SpawnPlayer()
    {
        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (index >= _characterPrefabs.Length) index = 0;

        GameObject playerInstance = Instantiate(_characterPrefabs[index], _spawnPoint.position, Quaternion.identity);

        Debug.Log("Đã sinh ra nhân vật: " + playerInstance.name);

        if (_virtualCamera != null)
        {
            _virtualCamera.Follow = playerInstance.transform;

            Debug.Log("Đã gán Camera Follow vào: " + playerInstance.name);
        }
        else
        {
            Debug.LogError("LỖI: Không tìm thấy Cinemachine Camera trong Scene!");
        }
    }
}