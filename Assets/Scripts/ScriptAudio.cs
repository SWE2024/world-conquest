using UnityEngine;

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
            if (isMuted) music.volume = Preferences.CurrentVolume;
            else music.volume = 0f;

            isMuted = !isMuted;
        }
    }

    public static void IncreaseVol()
    {
        if (GameObject.Find("Music").GetComponent<AudioSource>().volume < 1f)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume += 0.1f;
            Preferences.CurrentVolume = GameObject.Find("Music").GetComponent<AudioSource>().volume;
        }
    }

    public static void DecreaseVol()
    {
        if (GameObject.Find("Music").GetComponent<AudioSource>().volume > 0f)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume -= 0.1f;
            Preferences.CurrentVolume = GameObject.Find("Music").GetComponent<AudioSource>().volume;
        }
    }
}
