using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour, IManager
{
    [SerializeField] Canvas mainSceneUI;
    [SerializeField] Canvas playSceneUI;
    [SerializeField] Canvas settingsWindow;
    [SerializeField] Canvas loadWindow;
    [SerializeField] Slider loadSlider;
    [SerializeField] TextMeshProUGUI loadText;
    int sceneIndex;
    Canvas otherWindow;

    public void StartManager()
    {
        List<Canvas> canvases = new List<Canvas>();
        canvases.Add(mainSceneUI);
        canvases.Add(playSceneUI);
        canvases.Add(settingsWindow);
        canvases.Add(loadWindow);
        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.GetComponent<IUserInterface>()?.StartUI();
            canvas.enabled = false;
        }
        CanvasGroup group = settingsWindow.GetComponent<CanvasGroup>();
        group.alpha = 0;
    }

    public void CloseLoadWindow(int index)
    {
        Time.timeScale = 1;
        sceneIndex = index;
        mainSceneUI.enabled = sceneIndex is 1;
        playSceneUI.enabled = sceneIndex > 1;
        loadWindow.enabled = false;
    }

    public void LoadScene(int progress, string action)
    {
        Time.timeScale = 0;
        loadWindow.enabled = true;
        mainSceneUI.enabled = playSceneUI.enabled = false;
        loadSlider.value = progress;
        loadText.text = action + progress + " %";
    }

    public void OpenSettings(Canvas _otherWindow)
    {
        otherWindow = _otherWindow;
        OpenSettings(true);
    }

    public void OpenSettings(bool isOpen)
    {
        if (isOpen)
            StartCoroutine(SmoothTransition(settingsWindow, otherWindow));
        else
            StartCoroutine(SmoothTransition(otherWindow, settingsWindow));
    }

    public IEnumerator SmoothTransition(Canvas openingWindow, Canvas closingWindow)
    {
        CanvasGroup openingGroup = openingWindow.GetComponent<CanvasGroup>();
        CanvasGroup closingGroup = closingWindow.GetComponent<CanvasGroup>();
        openingWindow.enabled = true;
        while (openingGroup?.alpha < 1 || closingGroup?.alpha > 0)
        {
            openingGroup.alpha += Time.unscaledDeltaTime * 5f;
            closingGroup.alpha -= Time.unscaledDeltaTime * 5f;
            yield return null;
        }
        closingWindow.enabled = false;
    }

    public IEnumerator SmoothOperation(bool isOpen, Canvas targetWindow)
    {
        CanvasGroup targetGroup = targetWindow.GetComponent<CanvasGroup>();
        if (isOpen)
        {
            targetWindow.enabled = true;
            while (targetGroup?.alpha < 1)
            {
                targetGroup.alpha += Time.unscaledDeltaTime * 5f;
                yield return null;
            }
        }
        else
        {
            while (targetGroup?.alpha > 0)
            {
                targetGroup.alpha -= Time.unscaledDeltaTime * 5f;
                yield return null;
            }
            targetWindow.enabled = false;
        }
    }
}
