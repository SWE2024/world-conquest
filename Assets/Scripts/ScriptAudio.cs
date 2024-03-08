using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptAudio : MonoBehaviour
{
    [SerializeField] AudioSource music;
    [SerializeField] GameObject menuSettings;
    float specifiedVolume;

    public AudioClip menu;
    public AudioClip game;

    void Awake()
    {
        DontDestroyOnLoad(music);
        DontDestroyOnLoad(menuSettings);
    }

    // Start is called before the first frame update
    void Start()
    {
        specifiedVolume = 0.15f;
        music.clip = menu;
        music.Play();

        Button volUp = GameObject.Find("VolumeUp").GetComponent<Button>();
        Button volDown = GameObject.Find("VolumeDown").GetComponent<Button>();

        volUp.onClick.AddListener(IncreaseVol);
        volDown.onClick.AddListener(DecreaseVol);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "SceneGame")
        {
            music.clip = game;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            float currentVolume = music.volume;
            if (currentVolume == 0f) music.volume = specifiedVolume;
            else music.volume = 0f;
        }
    }

    void IncreaseVol()
    {
        if (music.volume < 0.5f)
        {
            music.volume += 0.05f;
        }
    }

    void DecreaseVol()
    {
        if (music.volume > 0f)
        {
            music.volume -= 0.05f;
        }
    }
}
