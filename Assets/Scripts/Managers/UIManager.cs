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
    }

    public void GetSceneIndex(int index)
    {
        sceneIndex = index;
        mainSceneUI.enabled = sceneIndex == 1;
        playSceneUI.enabled = sceneIndex > 1;
        loadWindow.enabled = false;
    }

    public void LoadScene(float progress)
    {
        loadWindow.enabled = true;
        mainSceneUI.enabled = false;
        playSceneUI.enabled = false;
        loadSlider.value = progress;
        float percentages = progress * 100;
        loadText.text = "Loading " + (int)percentages + " %";
    }

    public void OpenSettins(bool isOpen)
    {
        settingsWindow.enabled = isOpen;
        if (sceneIndex == 1)
        {
            mainSceneUI.enabled = !isOpen;
            BroadcastMessages<bool>.SendMessage(Messages.PAUSE, isOpen);
        }
        else if (sceneIndex > 1)
            playSceneUI.enabled = !isOpen;
    }
}
