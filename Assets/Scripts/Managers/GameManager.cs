using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<CarData> carData;
    [SerializeField] GameSettings defaultSettings;
    public static GameManager gameManager { get; private set; }
    public static UIManager uiManager { get; private set; }
    public static AudioManager audioManager { get; private set; }
    public GameData gameData { get; private set; }
    public GameSettings gameSettings { get; private set; }
    public GameSettings DefaultSettings { get { return defaultSettings;} }
    public List<CarData> CarData { get { return carData; } }
    AsyncOperation load;
    Scene scene;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadData();
        LoadSettings();
        gameManager = this;
        uiManager = GetComponentInChildren<UIManager>();
        audioManager = GetComponentInChildren<AudioManager>();
        uiManager.StartManager();
        audioManager.StartManager();
        load = SceneManager.LoadSceneAsync(1);
        load.completed += LoadCompleted;
    }

    void OnEnable() => BroadcastMessages<bool>.AddListener(Messages.PAUSE, Pause);
    void OnDisable() => BroadcastMessages<bool>.RemoveListener(Messages.PAUSE, Pause);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && scene.buildIndex > 1 && Time.timeScale == 1)
            BroadcastMessages<bool>.SendMessage(Messages.PAUSE, true);
    }

    void LoadData()
    {
        GameData _gameData;
        _gameData.carData = carData[0];
        gameData = _gameData;
    }
    void LoadSettings()
    {
        GameSettings _gameSettings;
        if (PlayerPrefs.HasKey("Braking"))
        {
            _gameSettings.quality = (Quality)PlayerPrefs.GetInt("Quality");
            _gameSettings.lightsTumbler = (KeyCode)PlayerPrefs.GetInt("Lights tumbler");
            _gameSettings.braking = (KeyCode)PlayerPrefs.GetInt("Braking");
            _gameSettings.soundVolume = PlayerPrefs.GetFloat("Sound volume");
            _gameSettings.musicVolume = PlayerPrefs.GetFloat("Music volume");
            gameSettings = _gameSettings;
        }
        else
        {
            gameSettings = defaultSettings;
            SaveSettings();
        }
        QualitySettings.SetQualityLevel((int)gameSettings.quality);
    }
    void SaveSettings()
    {
        PlayerPrefs.SetInt("Quality", (int)gameSettings.quality);
        PlayerPrefs.SetInt("Lights tumbler", (int)gameSettings.lightsTumbler);
        PlayerPrefs.SetInt("Braking", (int)gameSettings.braking);
        PlayerPrefs.SetFloat("Sound volume", gameSettings.soundVolume);
        PlayerPrefs.SetFloat("Music volume", gameSettings.musicVolume);
        PlayerPrefs.Save();
    }
    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
        gameSettings = defaultSettings;
        SaveSettings();
    }

    void Pause(bool isPause)
    {
        Time.timeScale = isPause ? 0f : 1f;
        if (scene.buildIndex > 1)
        {
            Cursor.visible = isPause;
            Cursor.lockState = isPause ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void SetData(GameData _gameData) => gameData = _gameData;
    public void SetSettings(GameSettings _gameSettings)
    {
        gameSettings = _gameSettings;
        SaveSettings();
        QualitySettings.SetQualityLevel((int)gameSettings.quality);
    }

    public void LoadScene(int scene)
    {
        load = SceneManager.LoadSceneAsync(scene);
        load.completed += LoadCompleted;
        StartCoroutine(ProgressLoad());
    }
    void LoadCompleted(AsyncOperation load)
    {
        scene = SceneManager.GetActiveScene();
        BroadcastMessages<bool>.SendMessage(Messages.PAUSE, false);
        uiManager.GetSceneIndex(scene.buildIndex);
        load.completed -= LoadCompleted;
    }
    IEnumerator ProgressLoad()
    {
        while (!load.isDone)
        {
            uiManager.LoadScene(load.progress);
            yield return null;
        }
    }

    public void ExitGame()
    {
        SaveSettings();
        Application.Quit();
    }
}
