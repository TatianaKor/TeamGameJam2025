using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        continueButton.onClick.AddListener(ContinueGame);
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void ContinueGame()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    private void RestartGame()
    {
        //TODO
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0); //Main menu
    }
}
