using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(UIManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] List<CarData> carData;
    public static GameManager gameManager { get; private set; }
    public static UIManager uiManager { get; private set; }
    public GameData gameData { get; private set; }
    public GameSettings gameSettings { get; private set; }
    public List<CarData> CarData { get { return carData; } }
    AsyncOperation load;
    Scene scene;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameManager = this;
        uiManager = GetComponent<UIManager>();
        uiManager.StartManager();
        LoadGameData();
        LoadGameSettings();
        load = SceneManager.LoadSceneAsync(1);
        load.completed += LoadCompleted;
    }

    void LoadGameData()
    {
        GameData _gameData;
        _gameData.carData = carData[0];
        gameData = _gameData;
    }
    void LoadGameSettings()
    {
        GameSettings _gameSettings;
        if (PlayerPrefs.HasKey("Braking"))
        {
            _gameSettings.quality = PlayerPrefs.GetInt("Quality");
            _gameSettings.lightsTumbler = (KeyCode)PlayerPrefs.GetInt("Lights tumbler");
            _gameSettings.braking = (KeyCode)PlayerPrefs.GetInt("Braking");
            gameSettings = _gameSettings;
        }
        else
        {
            _gameSettings.quality = 1;
            _gameSettings.lightsTumbler = KeyCode.LeftShift;
            _gameSettings.braking = KeyCode.Space;
            gameSettings = _gameSettings;
            PlayerPrefs.SetInt("Quality", gameSettings.quality);
            PlayerPrefs.SetInt("Lights tumbler", (int)gameSettings.lightsTumbler);
            PlayerPrefs.SetInt("Braking", (int)gameSettings.braking);
            PlayerPrefs.Save();
        }
        QualitySettings.SetQualityLevel(gameSettings.quality);
    }
    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
        GameSettings _gameSettings;
        _gameSettings.quality = 1;
        _gameSettings.lightsTumbler = KeyCode.LeftShift;
        _gameSettings.braking = KeyCode.Space;
        gameSettings = _gameSettings;
        PlayerPrefs.SetInt("Quality", gameSettings.quality);
        PlayerPrefs.SetInt("Lights tumbler", (int)gameSettings.lightsTumbler);
        PlayerPrefs.SetInt("Braking", (int)gameSettings.braking);
        PlayerPrefs.Save();
    }

    void OnEnable()
    {
        BroadcastMessages<bool>.AddListener(Messages.PAUSE, Pause);
    }
    void OnDisable()
    {
        BroadcastMessages<bool>.RemoveListener(Messages.PAUSE, Pause);
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

    public void SetGameData(GameData _gameData) => gameData = _gameData;
    public void SetGameSettings(GameSettings _gameSettings)
    {
        gameSettings = _gameSettings;
        QualitySettings.SetQualityLevel(gameSettings.quality);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && scene.buildIndex > 1 && Time.timeScale == 1)
            BroadcastMessages<bool>.SendMessage(Messages.PAUSE, true);
    }

    public void ExitGame()
    {
        PlayerPrefs.SetInt("Quality", gameSettings.quality);
        PlayerPrefs.SetInt("Lights tumbler", (int)gameSettings.lightsTumbler);
        PlayerPrefs.SetInt("Braking", (int)gameSettings.braking);
        PlayerPrefs.Save();
        Application.Quit();
    }
}
