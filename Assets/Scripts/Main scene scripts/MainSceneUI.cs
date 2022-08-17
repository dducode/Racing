using UnityEngine;

public class MainSceneUI : MonoBehaviour, IUserInterface
{
    [SerializeField] GameObject mainWindow;
    [SerializeField] GameObject selectWindow;

    public void StartUI() => selectWindow.SetActive(false);

    void Update() => mainWindow.SetActive(!selectWindow.activeSelf);

    public void StartGame() => GameManager.gameManager.LoadScene(2);
    public void Exit() => GameManager.gameManager.ExitGame();
    public void SelectCar() => selectWindow.SetActive(true);
}