using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] private bool PlayMusic = true;
    [HideInInspector] private float MusicVolume = 1f;
    [SerializeField] private AudioSource rapAudioSource;

    public AudioSource stepSounds;
    public AudioSource deathSound;
    public AudioSource landingSound;
    public AudioSource pickupSound;
    public AudioSource spitSound;
    public AudioSource spitSplashSound;
    public AudioSource bubblePopSound;
    
    private PlayerController player;
    private GameObject[] pickableObjects;
    private WinPanelController winPanel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += FindObjects;
    }

    private void FindObjects(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        pickableObjects = GameObject.FindGameObjectsWithTag("Pickable");
        var winPanels = Resources.FindObjectsOfTypeAll<WinPanelController>();
        if(winPanels.Length > 0)
        {
            winPanel = winPanels[0];
        }
    }

    public void RestartLevel()
    {
        if (player == null)
        {
            return;
        }

        player.Restart();
        foreach (var pickableObject in pickableObjects)
        {
            pickableObject.SetActive(true);
        }
    }

    public void SetPlayMusic(bool value)
    {
        PlayMusic = value;
        rapAudioSource.mute = !value;
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        rapAudioSource.volume = value;
        stepSounds.volume = value;
    }

    public bool GetPlayMusic()
    {
        return PlayMusic;
    }

    public float GetMusicVolume()
    {
        return MusicVolume;
    }

    public void WinGame()
    {
        winPanel.gameObject.SetActive(true);
    }
}
