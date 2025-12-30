using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] respawnPoints;
    public                   Transform[] Points => respawnPoints;
}