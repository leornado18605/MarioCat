using UnityEngine;

public class FlagChange : MonoBehaviour
{
    [Header("Detect")]
    [SerializeField] private string playerTag = "Player";

    [Header("Target")]
    [SerializeField] private SpriteRenderer flagRenderer; // SpriteRenderer của GameObject

    [Header("Sprites")]
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteB;

    private bool changed;

    private void Awake()
    {
        if (flagRenderer && spriteA)
            flagRenderer.sprite = spriteA;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (changed) return;
        if (!other.CompareTag(playerTag)) return;

        ChangeFlag();
    }

    private void ChangeFlag()
    {
        changed = true;
        AudioManager.I?.PlayCheckpoint();
        if (flagRenderer && spriteB)
            flagRenderer.sprite = spriteB;
    }
}