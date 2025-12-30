using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPersist : MonoBehaviour
{
    public static UIPersist I { get; private set; }

    [Header("Finish UI")]
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private TMP_Text countdownText;

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        ResetFinishUI();
        SceneManager.sceneLoaded += (_, __) => ResetFinishUI();
    }

    private void OnDestroy()
    {
        if (I == this)
            SceneManager.sceneLoaded -= (_, __) => ResetFinishUI();
    }

    public void ResetFinishUI()
    {
        if (finishPanel) finishPanel.SetActive(false);
        if (countdownText) countdownText.text = "";
    }

    public void ShowCountdown(float seconds, Action onDone)
    {
        StopAllCoroutines();
        StartCoroutine(CountdownRoutine(seconds, onDone));
    }

    private IEnumerator CountdownRoutine(float seconds, Action onDone)
    {
        if (finishPanel) finishPanel.SetActive(true);

        float t = seconds;
        while (t > 0f)
        {
            if (countdownText) countdownText.text = Mathf.CeilToInt(t).ToString();
            yield return null;
            t -= Time.unscaledDeltaTime;
        }

        if (countdownText) countdownText.text = "0";
        onDone?.Invoke();
    }
}