using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanelController : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        });
    }

    private void OnEnable()
    {
        InputSystem.actions.FindActionMap("Player").Disable();
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
