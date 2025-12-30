using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SetupManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject panel1;

    [Header("Buttons")]
    [SerializeField] private Button btnOpen;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnSoundOn;
    [SerializeField] private Button btnSoundOff;

    [Header("Optional Text")]
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private TMP_Text soundText;

    private bool paused;
    private bool soundOn;

    private void Awake()
    {
        if (btnOpen) btnOpen.onClick.AddListener(TogglePanel);
        if (btnClose) btnClose.onClick.AddListener(ClosePanel);
        if (btnRestart) btnRestart.onClick.AddListener(Restart);

        if (btnPause) btnPause.onClick.AddListener(PauseGame);
        if (btnContinue) btnContinue.onClick.AddListener(ContinueGame);

        if (btnSoundOn) btnSoundOn.onClick.AddListener(SoundOff);
        if (btnSoundOff) btnSoundOff.onClick.AddListener(SoundOn);

    }

    private void Start()
    {
        soundOn = AudioListener.volume > 0.001f;
        if (panel) panel.SetActive(false);
        ApplyUI();
    }

    private void OnDestroy()
    {
        if (btnOpen) btnOpen.onClick.RemoveListener(TogglePanel);
        if (btnClose) btnClose.onClick.RemoveListener(ClosePanel);
        if (btnRestart) btnRestart.onClick.RemoveListener(Restart);

        if (btnPause) btnPause.onClick.RemoveListener(PauseGame);
        if (btnContinue) btnContinue.onClick.RemoveListener(ContinueGame);

        if (btnSoundOn) btnSoundOn.onClick.RemoveListener(SoundOff);
        if (btnSoundOff) btnSoundOff.onClick.RemoveListener(SoundOn);

    }

    public void TogglePanel()
    {
        if (!panel) return;
        panel.SetActive(!panel.activeSelf);
        ApplyUI();
    }

    public void ClosePanel()
    {
        if (!panel) return;
        panel.SetActive(false);
    }

    public void PauseGame()
    {
        paused = true;
        Time.timeScale = 0f;
        ApplyUI();
    }

    public void ContinueGame()
    {
        paused = false;
        Time.timeScale = 1f;
        ApplyUI();
    }

    public void SoundOn()
    {
        soundOn = true;
        AudioListener.volume = 1f;
        ApplyUI();
    }

    public void SoundOff()
    {
        soundOn = false;
        AudioListener.volume = 0f;
        ApplyUI();
    }

   // public void Restart() { Time.timeScale = 1f; if (panel) panel.SetActive(false); if (panel1) panel1.SetActive(false); var playerPersist = Object.FindFirstObjectByType<PlayerPersist>(); if (playerPersist) Destroy(playerPersist.gameObject); var uiPersist = Object.FindFirstObjectByType<UIPersist>(); if (uiPersist) Destroy(uiPersist.gameObject); int idx = SceneManager.GetActiveScene().buildIndex; if (SceneTransition.I != null) SceneTransition.I.LoadScene(idx); else SceneManager.LoadScene(idx); }

   public void Restart()
   {
       Time.timeScale = 1f;

       if (panel) panel.SetActive(false);
       if (panel1) panel1.SetActive(false);

       var player = Object.FindFirstObjectByType<PlayerHealth>();
       if (player)
           player.ResetPlayer();

       int idx = SceneManager.GetActiveScene().buildIndex;

       if (SceneTransition.I != null)
           SceneTransition.I.LoadScene(idx);
       else
           SceneManager.LoadScene(idx);
   }

    private void ApplyUI()
    {
        if (btnPause) btnPause.gameObject.SetActive(!paused);
        if (btnContinue) btnContinue.gameObject.SetActive(paused);

        if (btnSoundOn) btnSoundOn.gameObject.SetActive(soundOn);
        if (btnSoundOff) btnSoundOff.gameObject.SetActive(!soundOn);

        if (pauseText) pauseText.text = paused ? "Continue" : "Pause";
        if (soundText) soundText.text = soundOn ? "Sound: ON" : "Sound: OFF";
    }

    public void LoadSelectCharacter()
    {
        Time.timeScale = 1f;

        if (SceneTransition.I != null)
            SceneTransition.I.LoadScene("SelectCharacter");
        else
            SceneManager.LoadScene("SelectCharacter");
    }

}