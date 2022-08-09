using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneCamera : MonoBehaviour
{
    [SerializeField] List<Transform> carPosition;
    int carIndex;

    void OnEnable() => BroadcastMessages<int>.AddListener(Messages.UPDATE_VIEW, UpdateView);
    void OnDisable() => BroadcastMessages<int>.RemoveListener(Messages.UPDATE_VIEW, UpdateView);

    void Start()
    {
        List<CarData> carData = GameManager.gameManager.CarData;
        GameData gameData = GameManager.gameManager.gameData;
        for (int i = 0; i < carData.Count; i++)
            if (gameData.carData == carData[i])
                carIndex = i;
        Vector3 cameraPos = transform.position;
        cameraPos.x = carPosition[carIndex].position.x;
        transform.position = cameraPos;
    }

    public void UpdateView(int index) => carIndex = index;

    void LateUpdate()
    {
        Vector3 cameraPos = transform.position;
        cameraPos.x = Mathf.Lerp(cameraPos.x, carPosition[carIndex].position.x, Time.deltaTime * 15);
        transform.position = cameraPos;
    }
}
