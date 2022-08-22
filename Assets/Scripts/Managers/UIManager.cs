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
        mainSceneUI.enabled = sceneIndex == 1;
        playSceneUI.enabled = sceneIndex > 1;
        loadWindow.enabled = false;
    }

    public void LoadScene(int progress, string action)
    {
        Time.timeScale = 0;
        loadWindow.enabled = true;
        mainSceneUI.enabled = false;
        playSceneUI.enabled = false;
        loadSlider.value = progress;
        loadText.text = action + progress + " %";
    }

    public void OpenSettins(bool isOpen)
    {
        StartCoroutine(OpenWindow(isOpen, settingsWindow));
        if (sceneIndex == 1)
        {
            mainSceneUI.enabled = !isOpen;
            Time.timeScale = isOpen ? 0 : 1;
        }
        else if (sceneIndex > 1)
            playSceneUI.enabled = !isOpen;
    }
    IEnumerator OpenWindow(bool isOpen, Canvas canvas)
    {
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        if (isOpen)
        {
            canvas.enabled = isOpen;
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime * 5f;
                yield return null;
            }
        }
        else if (canvasGroup is not null)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.unscaledDeltaTime * 5f;
                yield return null;
            }
            canvas.enabled = isOpen;
        }
    }
}
