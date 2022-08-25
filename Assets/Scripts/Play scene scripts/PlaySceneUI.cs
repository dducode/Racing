using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaySceneUI : MonoBehaviour, IUserInterface
{
    [SerializeField] Canvas pauseWindow;
    [SerializeField] GameObject fps;
    [SerializeField] TextMeshProUGUI fpsText;
    [SerializeField] TextMeshProUGUI visualSpeed;
    [SerializeField] TextMeshProUGUI visualTransmission;
    [SerializeField] Transform tahometerArrow;
    [SerializeField] float timeFPS = 0.5f;
    CanvasGroup pauseGroup;
    float currentTimeFPS;
    float meanFPS;
    int frameCount;
    Vector3 rotationTemp;

    void OnEnable()
    {
        BroadcastMessages<float, int, float>.AddListener(Messages.UPDATE_VIEW, UpdateTahometer);
        BroadcastMessages<bool>.AddListener(Messages.PAUSE, Pause);
        currentTimeFPS = timeFPS;
    }
    void OnDisable()
    {
        BroadcastMessages<float, int, float>.RemoveListener(Messages.UPDATE_VIEW, UpdateTahometer);
        BroadcastMessages<bool>.RemoveListener(Messages.PAUSE, Pause);
        tahometerArrow.rotation = Quaternion.Euler(rotationTemp);
        visualSpeed.text = "000";
        visualTransmission.text = "1";
    }

    public void StartUI()
    {
        pauseWindow.enabled = false;
        pauseGroup = pauseWindow.GetComponent<CanvasGroup>();
        pauseGroup.alpha = 0;
        fps.SetActive(false);
        rotationTemp = tahometerArrow.rotation.eulerAngles;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            fps.SetActive(!fps.activeSelf);
        if (fps.activeSelf)
        {
            if (currentTimeFPS > 0f)
            {
                currentTimeFPS -= Time.unscaledDeltaTime;
                meanFPS += 1 / Time.unscaledDeltaTime;
                frameCount++;
            }
            else
            {
                meanFPS /= frameCount;
                fpsText.text = "FPS: " + (int)meanFPS;
                currentTimeFPS = timeFPS;
                meanFPS = 0f;
                frameCount = 0;
            }
        }
    }
    
    public void UpdateTahometer(float speed, int transmission, float engineSpeed)
    {
        switch (speed)
        {
            case < 10f:
                visualSpeed.text = "00" + (int)speed;
                break;
            case < 100f:
                visualSpeed.text = "0" + (int)speed;
                break;
            default:
                visualSpeed.text = "" + (int)speed;
                break;
        }
        if (transmission > 0)
            visualTransmission.text = transmission.ToString();
        else
            visualTransmission.text = "R";
        Quaternion rotation = Quaternion.Euler(0, 0, -engineSpeed / 37.736f + rotationTemp.z);
        tahometerArrow.rotation = Quaternion.Slerp(tahometerArrow.rotation, rotation, Time.deltaTime * 5f);
    }

    void Pause(bool isPause)
    {
        if (GetComponent<Canvas>().enabled)
            StartCoroutine(GameManager.uiManager.SmoothOperation(isPause, pauseWindow));
    }

    public void OpenSettings() => GameManager.uiManager.OpenSettings(pauseWindow);
    public void Continue() => BroadcastMessages<bool>.SendMessage(Messages.PAUSE, false);
    public void MainMenu()
    {
        pauseWindow.enabled = false;
        pauseGroup.alpha = 0;
        GameManager.gameManager.LoadScene(1);
    }
    public void Exit() => GameManager.gameManager.ExitGame();
}
