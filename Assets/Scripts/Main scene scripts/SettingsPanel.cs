using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] TMP_Dropdown qualitySelect;
    [SerializeField] TextMeshProUGUI lightsTumblerButton;
    [SerializeField] TextMeshProUGUI brakingButton;
    GameSettings gameSettings;
    Event e;

    void Awake()
    {
        gameSettings = GameManager.gameManager.gameSettings;
        qualitySelect.value = gameSettings.quality;
        qualitySelect.RefreshShownValue();
        brakingButton.text = gameSettings.braking.ToString();
        lightsTumblerButton.text = gameSettings.lightsTumbler.ToString();
    }

    void OnGUI() => e = Event.current;

    public void SetQuality(int count)
    {
        gameSettings = GameManager.gameManager.gameSettings;
        gameSettings.quality = count;
        qualitySelect.value = gameSettings.quality;
        qualitySelect.RefreshShownValue();
        GameManager.gameManager.SetGameSettings(gameSettings);
    }

    public void ResetSettings()
    {
        GameManager.gameManager.ResetSettings();
        gameSettings = GameManager.gameManager.gameSettings;
        qualitySelect.value = gameSettings.quality;
        qualitySelect.RefreshShownValue();
        brakingButton.text = gameSettings.braking.ToString();
        lightsTumblerButton.text = gameSettings.lightsTumbler.ToString();
    }

    public void SetButton(string buttonName)
    {
        StopAllCoroutines();
        StartCoroutine(WaitPlayerInput(buttonName));
    }

    IEnumerator WaitPlayerInput(string buttonName)
    {
        yield return new WaitWhile(() => e == null || !e.isKey || e.keyCode == KeyCode.None);
        gameSettings = GameManager.gameManager.gameSettings;
        switch (buttonName)
        {
            case "Braking":
                gameSettings.braking = e.keyCode;
                brakingButton.text = gameSettings.braking.ToString();
                break;
            case "Lights tumbler":
                gameSettings.lightsTumbler = e.keyCode;
                lightsTumblerButton.text = gameSettings.lightsTumbler.ToString();
                break;
        }
        GameManager.gameManager.SetGameSettings(gameSettings);
    }
}
