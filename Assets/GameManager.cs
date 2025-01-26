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
    
    private PlayerController player;
    private GameObject[] pickableObjects;

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
}
