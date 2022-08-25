using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MainSceneUI : MonoBehaviour, IUserInterface
{
    [SerializeField] Canvas selectWindow;
    [SerializeField] CarInfo carInfo;
    [SerializeField] Canvas outputField;
    [SerializeField] Button forward;
    [SerializeField] Button back;
    Canvas mainWindow;
    List<CarData> carData;
    GameData gameData;
    int _index;
    public int index { get { return _index; } }

    public void StartUI()
    {
        mainWindow = GetComponent<Canvas>();
        List<Canvas> windows = new List<Canvas>();
        windows.Add(selectWindow);
        windows.Add(outputField);
        foreach (Canvas window in windows)
        {
            window.enabled = false;
            window.GetComponent<CanvasGroup>().alpha = 0;
        }
        carData = GameManager.gameManager.CarData;
        gameData = GameManager.gameManager.gameData;
        for (int i = 0; i < carData.Count; i++)
            if (gameData.carData == carData[i])
                _index = i;
        UpdateData();
        back.interactable = _index > 0;
        forward.interactable = _index < carData.Count - 1;
    }

    public void Forward()
    {
        back.interactable = true;
        _index++;
        _index = Mathf.Clamp(_index, 0, carData.Count);
        forward.interactable = _index < carData.Count - 1;
        UpdateData();
    }
    public void Back()
    {
        forward.interactable = true;
        _index--;
        _index = Mathf.Clamp(_index, 0, carData.Count);
        back.interactable = _index > 0;
        UpdateData();
    }

    public void SelectCar() => 
        StartCoroutine(GameManager.uiManager.SmoothTransition(selectWindow, mainWindow));

    public void Complete()
    {
        gameData = GameManager.gameManager.gameData;
        gameData.carData = carData[_index];
        GameManager.gameManager.SetData(gameData);
        StartCoroutine(GameManager.uiManager.SmoothTransition(mainWindow, selectWindow));
    }

    public void Info() => 
        StartCoroutine(GameManager.uiManager.SmoothOperation(!outputField.enabled, outputField));

    void UpdateData()
    {
        carInfo.carName.text = carData[_index].carName;
        carInfo.maxMotorTorque.text = carData[_index].maxMotorTorque.ToString();
        carInfo.maxBrakingTorque.text = carData[_index].maxBrakingTorque.ToString();
        carInfo.mass.text = carData[_index].mass.ToString();
        carInfo.transmission.text = carData[_index].transmission.ToString();
        BroadcastMessages<int>.SendMessage(Messages.UPDATE_VIEW, _index);
    }

    public void StartGame() => GameManager.gameManager.LoadScene(2);
    public void Exit() => GameManager.gameManager.ExitGame();
    public void OpenSettings() => GameManager.uiManager.OpenSettings(GetComponent<Canvas>());
}

[System.Serializable]
public class CarInfo
{
    public TextMeshProUGUI carName;
    public TextMeshProUGUI maxMotorTorque;
    public TextMeshProUGUI maxBrakingTorque;
    public TextMeshProUGUI mass;
    public TextMeshProUGUI transmission;
}