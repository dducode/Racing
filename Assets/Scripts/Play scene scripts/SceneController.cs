using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField, Tooltip("Координаты точки респауна игрока")] 
    Vector3 carPosition;
    Behaviours behaviours;

    void Awake()
    {
        GameData gameData = GameManager.gameManager.gameData;
        CarData carData = gameData.carData;
        behaviours = new Behaviours();
        GameObject car = Instantiate(carData.car);
        car.name = carData.carName;
        car.transform.position = carPosition;
        behaviours.carMovement = car.GetComponent<CarMovement>();
        behaviours.firstPersonCamera = car.GetComponentInChildren<FirstPersonCameraMove>();
        behaviours.thirdPersonCamera = car.GetComponentInChildren<ThirdPersonCameraMove>();
    }

    void OnEnable() => BroadcastMessages<bool>.AddListener(Messages.PAUSE, Pause);
    void OnDisable() => BroadcastMessages<bool>.RemoveListener(Messages.PAUSE, Pause);
    
    public void Pause(bool isPause)
    {
        behaviours.carMovement.enabled = !isPause;
        behaviours.thirdPersonCamera.enabled = !isPause;
    }
}

public class Behaviours
{
    public CarMovement carMovement;
    public FirstPersonCameraMove firstPersonCamera;
    public ThirdPersonCameraMove thirdPersonCamera;
}
