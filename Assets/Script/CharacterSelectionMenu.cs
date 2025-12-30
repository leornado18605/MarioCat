using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterSelectionMenu : MonoBehaviour
{
    [Header("Root UI")]
    [SerializeField] private GameObject selectRoot;
    [SerializeField] private RectTransform selectWindow;

    [Header("Panels")]
    [SerializeField] private GameObject[] characterPanels;

    [Header("Pick Buttons")]
    [SerializeField] private Button[] pickButtons;

    [Header("Tween - Pick")]
    [SerializeField] private float selectedScale = 1.15f;
    [SerializeField] private float unselectedScale = 0.9f;
    [SerializeField] private float selectedAlpha = 1f;
    [SerializeField] private float unselectedAlpha = 0.45f;
    [SerializeField] private float pickTweenTime = 0.2f;

    [Header("Tween - Open/Close")]
    [SerializeField] private float windowScaleFrom = 0.9f;
    [SerializeField] private float windowTweenTime = 0.25f;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Level1";

    private int selectedIndex;
    private bool isTransitioning;
    private CanvasGroup rootCg;
    private CanvasGroup windowCg;

    private void Start()
    {
        selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        if (selectRoot)
        {
            rootCg = selectRoot.GetComponent<CanvasGroup>();
            if (!rootCg) rootCg = selectRoot.AddComponent<CanvasGroup>();
            selectRoot.SetActive(false);
        }

        if (selectWindow)
        {
            windowCg = selectWindow.GetComponent<CanvasGroup>();
            if (!windowCg) windowCg = selectWindow.gameObject.AddComponent<CanvasGroup>();
        }

        ApplySelectionVisual(selectedIndex, true);
        ShowPanel(selectedIndex);
    }

    public void OpenSelect()
    {
        if (!selectRoot || !selectWindow || isTransitioning) return;
        isTransitioning = true;

        selectRoot.SetActive(true);

        rootCg.DOKill();
        selectWindow.DOKill();
        windowCg.DOKill();

        rootCg.alpha = 1f;
        selectWindow.localScale = Vector3.one * windowScaleFrom;
        windowCg.alpha = 0f;

        ShowPanel(selectedIndex);
        ApplySelectionVisual(selectedIndex, true);

        selectWindow.DOScale(1f, windowTweenTime).SetEase(Ease.OutBack);
        windowCg.DOFade(1f, windowTweenTime).OnComplete(() => isTransitioning = false);
    }

    public void CloseSelect()
    {
        if (!selectRoot || !selectWindow || isTransitioning) return;
        isTransitioning = true;

        selectWindow.DOKill();
        windowCg.DOKill();

        selectWindow.DOScale(windowScaleFrom, windowTweenTime).SetEase(Ease.InBack);
        windowCg.DOFade(0f, windowTweenTime).OnComplete(() =>
        {
            selectRoot.SetActive(false);
            isTransitioning = false;
        });
    }

    public void PickCharacter(int index)
    {
        if (isTransitioning) return;
        selectedIndex = index;
        ApplySelectionVisual(selectedIndex, false);
        ShowPanel(selectedIndex);
    }

    public void ChooseAndPlay()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }

    private void ShowPanel(int index)
    {
        if (characterPanels == null) return;
        for (int i = 0; i < characterPanels.Length; i++)
            if (characterPanels[i]) characterPanels[i].SetActive(i == index);
    }

    private void ApplySelectionVisual(int index, bool instant)
    {
        if (pickButtons == null) return;

        for (int i = 0; i < pickButtons.Length; i++)
        {
            var btn = pickButtons[i];
            if (!btn) continue;

            var t = btn.transform;
            var cg = btn.GetComponent<CanvasGroup>();
            if (!cg) cg = btn.gameObject.AddComponent<CanvasGroup>();

            bool selected = (i == index);
            float targetScale = selected ? selectedScale : unselectedScale;
            float targetAlpha = selected ? selectedAlpha : unselectedAlpha;

            t.DOKill();
            cg.DOKill();

            if (instant)
            {
                t.localScale = Vector3.one * targetScale;
                cg.alpha = targetAlpha;
            }
            else
            {
                t.DOScale(targetScale, pickTweenTime).SetEase(Ease.OutBack);
                cg.DOFade(targetAlpha, pickTweenTime);
            }

            btn.interactable = selected;
        }
    }
}