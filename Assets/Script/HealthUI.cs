using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image[]    hearts;
    [SerializeField] private GameObject losePanel;

    public Image[] Hearts => hearts;

    private void Awake()
    {
        if (losePanel) losePanel.SetActive(false);
    }

    public void SetHearts(int hp)
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++) if (hearts[i]) hearts[i].enabled = i < hp;
    }

    public void ShowLose()
    {
        if (losePanel) losePanel.SetActive(true);
    }
}