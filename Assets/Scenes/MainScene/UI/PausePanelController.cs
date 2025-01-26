using UnityEngine;
using UnityEngine.InputSystem;
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
        InputSystem.actions.FindActionMap("Player").Disable();
    }

    private void ContinueGame()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        InputSystem.actions.FindActionMap("Player").Enable();
    }

    private void RestartGame()
    {
        GameManager.Instance.RestartLevel();
        ContinueGame();
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0); //Main menu
    }
}
