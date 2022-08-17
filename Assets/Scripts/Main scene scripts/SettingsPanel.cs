using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SettingsPanel : MonoBehaviour, IUserInterface
{
    [SerializeField] TMP_Dropdown qualitySelect;
    [SerializeField] TextMeshProUGUI lightsTumblerButton;
    [SerializeField] TextMeshProUGUI brakingButton;
    [SerializeField] Slider soundVolume;
    [SerializeField] Slider musicVolume;
    [SerializeField] Button resetSettings;
    GameSettings gameSettings;
    Event e;

    public void StartUI()
    {
        gameSettings = GameManager.gameManager.gameSettings;
        qualitySelect.value = (int)gameSettings.quality;
        qualitySelect.RefreshShownValue();
        brakingButton.text = gameSettings.braking.ToString();
        lightsTumblerButton.text = gameSettings.lightsTumbler.ToString();
        soundVolume.value = gameSettings.soundVolume;
        musicVolume.value = gameSettings.musicVolume;
    }

    void OnGUI() => e = Event.current;

    public void SetQuality(int count)
    {
        gameSettings = GameManager.gameManager.gameSettings;
        gameSettings.quality = (Quality)count;
        qualitySelect.value = (int)gameSettings.quality;
        qualitySelect.RefreshShownValue();
        GameManager.gameManager.SetSettings(gameSettings);
        resetSettings.interactable = true;
    }

    public void SetSoundVolume(float value)
    {
        gameSettings = GameManager.gameManager.gameSettings;
        gameSettings.soundVolume = value;
        GameManager.gameManager.SetSettings(gameSettings);
        resetSettings.interactable = true;
    }

    public void SetMusicVolume(float value)
    {
        gameSettings = GameManager.gameManager.gameSettings;
        gameSettings.musicVolume = value;
        GameManager.gameManager.SetSettings(gameSettings);
        resetSettings.interactable = true;
    }

    public void ResetSettings()
    {
        GameManager.gameManager.ResetSettings();
        gameSettings = GameManager.gameManager.gameSettings;
        qualitySelect.value = (int)gameSettings.quality;
        qualitySelect.RefreshShownValue();
        brakingButton.text = gameSettings.braking.ToString();
        lightsTumblerButton.text = gameSettings.lightsTumbler.ToString();
        soundVolume.value = gameSettings.soundVolume;
        musicVolume.value = gameSettings.musicVolume;
        resetSettings.interactable = false;
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
        GameManager.gameManager.SetSettings(gameSettings);
    }
}
