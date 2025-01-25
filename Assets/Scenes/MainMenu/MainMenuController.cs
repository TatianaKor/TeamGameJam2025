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

        playMusicToggle.isOn = GameManager.Instance.PlayMusic;
        musicVolumeSlider.value = GameManager.Instance.MusicVolume;

        playMusicToggle.onValueChanged.AddListener((value) =>
        {
            GameManager.Instance.PlayMusic = value;
        });
        musicVolumeSlider.onValueChanged.AddListener((value) =>
        {
            GameManager.Instance.MusicVolume = value;
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
