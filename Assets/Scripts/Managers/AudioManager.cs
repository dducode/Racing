using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IManager
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioSource musicSource;

    public void StartManager()
    {
        GameSettings gameSettings = GameManager.gameManager.gameSettings;
        soundSource.volume = gameSettings.soundVolume;
        musicSource.volume = gameSettings.musicVolume;
    }
}
