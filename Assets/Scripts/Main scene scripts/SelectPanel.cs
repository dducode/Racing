using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SelectPanel : MonoBehaviour
{
    [SerializeField] CarInfo carInfo;
    [SerializeField] GameObject outputField;
    [SerializeField] Button forward;
    [SerializeField] Button back;
    List<CarData> carData;
    GameData gameData;
    int _index;
    public int index { get { return _index; } }

    void Awake()
    {
        carData = GameManager.gameManager.CarData;
        gameData = GameManager.gameManager.gameData;
        for (int i = 0; i < carData.Count; i++)
            if (gameData.carData == carData[i])
                _index = i;
    }

    void Start()
    {
        UpdateData();
        back.interactable = _index > 0;
        forward.interactable = _index < carData.Count - 1;
    }

    public void Forward()
    {
        if (!back.interactable)
            back.interactable = true;
        _index++;
        _index = Mathf.Clamp(_index, 0, carData.Count);
        if (_index == carData.Count - 1)
            forward.interactable = false;
        UpdateData();
    }
    public void Back()
    {
        if (!forward.interactable)
            forward.interactable = true;
        _index--;
        _index = Mathf.Clamp(_index, 0, carData.Count);
        if (_index == 0)
            back.interactable = false;
        UpdateData();
    }

    public void Complete()
    {
        gameData = GameManager.gameManager.gameData;
        gameData.carData = carData[_index];
        GameManager.gameManager.SetGameData(gameData);
        gameObject.SetActive(false);
    }

    public void Info() => outputField.SetActive(!outputField.activeSelf);

    void UpdateData()
    {
        carInfo.carName.text = carData[_index].carName;
        carInfo.maxMotorTorque.text = carData[_index].maxMotorTorque.ToString();
        carInfo.maxBrakingTorque.text = carData[_index].maxBrakingTorque.ToString();
        carInfo.mass.text = carData[_index].mass.ToString();
        carInfo.transmission.text = carData[_index].transmission.ToString();
        BroadcastMessages<int>.SendMessage(Messages.UPDATE_VIEW, _index);
    }
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
