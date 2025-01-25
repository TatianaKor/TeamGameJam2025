using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool PlayMusic = true;
    public float MusicVolume = 1f;

    private void Awake()
    {
        if(Instance != null)
        {
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
