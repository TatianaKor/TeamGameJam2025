using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public bool PlayMusic = true;
    [HideInInspector] public float MusicVolume = 1f;

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
}
