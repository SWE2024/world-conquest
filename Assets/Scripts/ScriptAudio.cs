using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// <c>ScriptAudio</c> handles the in-game audio.
/// </summary>
public class ScriptAudio : MonoBehaviour
{
    [SerializeField] AudioSource music;
    [SerializeField] bool isMuted = false;

    void Awake()
    {
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("GameMusic");
        if (musicObj.Length > 1)
        {
            Destroy(this.gameObject); // ensures only one music track plays at a time
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        music = GameObject.Find("Music").GetComponent<AudioSource>();
        music.volume = Preferences.CurrentVolume;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Canvas canvas = GameObject.Find("RenameCountry")?.GetComponent<Canvas>();
            if ((!SceneManager.GetActiveScene().name.Equals("SceneGame")) || (canvas != null && !canvas.enabled))
            {
                if (isMuted) music.volume = Preferences.CurrentVolume;
                else music.volume = 0f;

                isMuted = !isMuted;
            }
        }
    }

    /// <summary>
    /// <c>IncreaseVol</c> increases the game music.
    /// </summary>
    public static void IncreaseVol()
    {
        if (GameObject.Find("Music").GetComponent<AudioSource>().volume < 1f)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume += 0.1f;
            Preferences.CurrentVolume = GameObject.Find("Music").GetComponent<AudioSource>().volume;
        }
    }

    /// <summary>
    /// <c>DecreaseVol</c> decreases the game music.
    /// </summary>
    public static void DecreaseVol()
    {
        if (GameObject.Find("Music").GetComponent<AudioSource>().volume > 0f)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume -= 0.1f;
            Preferences.CurrentVolume = GameObject.Find("Music").GetComponent<AudioSource>().volume;
        }
    }
}
