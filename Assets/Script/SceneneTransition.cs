using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition I { get; private set; }

    [Header("Fade")]
    [SerializeField] private float fadeTime = 0.35f;
    [SerializeField] private int sortOrder = 999;

    private CanvasGroup canvasGroup;
    private bool isLoading;

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        EnsureOverlay();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void LoadScene(int buildIndex)
    {
        if (isLoading) return;
        EnsureOverlay();
        StartCoroutine(LoadRoutine(buildIndex, null));
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading) return;
        EnsureOverlay();
        StartCoroutine(LoadRoutine(-1, sceneName));
    }

    private void EnsureOverlay()
    {
        if (canvasGroup) return;

        var canvasGO = new GameObject("TransitionCanvas");
        canvasGO.transform.SetParent(transform, false);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var imgGO = new GameObject("Fade");
        imgGO.transform.SetParent(canvasGO.transform, false);

        var image = imgGO.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = true;

        var rt = imgGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        canvasGroup = imgGO.AddComponent<CanvasGroup>();
    }

    private IEnumerator LoadRoutine(int buildIndex, string sceneName)
    {
        isLoading = true;

        canvasGroup.DOKill();
        canvasGroup.blocksRaycasts = true;

        yield return canvasGroup.DOFade(1f, fadeTime).SetUpdate(true).WaitForCompletion();

        AsyncOperation op = (buildIndex >= 0)
            ? SceneManager.LoadSceneAsync(buildIndex)
            : SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone) yield return null;

        yield return canvasGroup.DOFade(0f, fadeTime).SetUpdate(true).WaitForCompletion();

        canvasGroup.blocksRaycasts = false;
        isLoading = false;
    }
}