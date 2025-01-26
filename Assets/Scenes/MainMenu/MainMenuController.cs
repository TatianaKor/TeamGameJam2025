using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Toggle playMusicToggle;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private GameObject loadingPanel;

    private void Start()
    {
        loadingPanel.SetActive(false);

        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);

        playMusicToggle.isOn = GameManager.Instance.GetPlayMusic();
        musicVolumeSlider.value = GameManager.Instance.GetMusicVolume();

        playMusicToggle.onValueChanged.AddListener((value) =>
        {
            GameManager.Instance.SetPlayMusic(value);
        });
        musicVolumeSlider.onValueChanged.AddListener((value) =>
        {
            GameManager.Instance.SetMusicVolume(value);
        });
    }

    private void StartGame()
    {
        SceneManager.LoadSceneAsync(1); //Main scene
        loadingPanel.SetActive(true);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
