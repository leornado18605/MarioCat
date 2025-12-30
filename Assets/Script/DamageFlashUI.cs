using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DamageFlashUI : MonoBehaviour
{
    public static DamageFlashUI I { get; private set; }

    [Header("Flash")]
    [SerializeField] private float duration = 1f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.6f;

    [Header("Optional (auto-created if null)")]
    [SerializeField] private Image flashImage;

    private CanvasGroup canvasGroup;
    private Coroutine flashCo;

    private void Awake()
    {
        if (I && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        EnsureOverlay();
        ResetAlpha();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (I == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        EnsureOverlay();
        ResetAlpha();
    }

    public void Play()
    {
        EnsureOverlay();

        if (flashCo != null) StopCoroutine(flashCo);
        flashCo = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetAlpha(maxAlpha);

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(maxAlpha, 0f, t / duration);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(0f);
        flashCo = null;
    }

    private void ResetAlpha() => SetAlpha(0f);

    private void EnsureOverlay()
    {
        if (flashImage)
        {
            canvasGroup = flashImage.GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = flashImage.gameObject.AddComponent<CanvasGroup>();
            flashImage.raycastTarget = false;
            return;
        }

        var canvasT = transform.Find("DamageFlashCanvas");
        GameObject canvasGO;

        if (!canvasT)
        {
            canvasGO = new GameObject("DamageFlashCanvas");
            canvasGO.transform.SetParent(transform, false);

            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10000;

            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        else
        {
            canvasGO = canvasT.gameObject;
        }

        var imgT = canvasGO.transform.Find("Flash");
        GameObject imgGO;

        if (!imgT)
        {
            imgGO = new GameObject("Flash");
            imgGO.transform.SetParent(canvasGO.transform, false);
        }
        else
        {
            imgGO = imgT.gameObject;
        }

        flashImage = imgGO.GetComponent<Image>();
        if (!flashImage) flashImage = imgGO.AddComponent<Image>();
        flashImage.color = new Color(1f, 0f, 0f, 0f);
        flashImage.raycastTarget = false;

        var rt = imgGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        canvasGroup = imgGO.GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = imgGO.AddComponent<CanvasGroup>();
    }

    private void SetAlpha(float a)
    {
        if (flashImage)
        {
            var c = flashImage.color;
            c.a = a;
            flashImage.color = c;
        }

        if (canvasGroup) canvasGroup.alpha = a;
    }
}