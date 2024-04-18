using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ScriptAudio : MonoBehaviour
{
    [SerializeField] AudioSource music;
    Button volUp;
    Button volDown;

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

        volUp = GameObject.Find("VolumeUp").GetComponent<Button>();
        volDown = GameObject.Find("VolumeDown").GetComponent<Button>();

        volUp.onClick.AddListener(IncreaseVol);
        volDown.onClick.AddListener(DecreaseVol);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (music.volume == 0f) music.volume = Preferences.CurrentVolume;
            else music.volume = 0f;
        }
    }

    void IncreaseVol()
    {
        if (music.volume < 0.5f)
        {
            music.volume += 0.05f;
            Preferences.CurrentVolume = music.volume;
        }
    }

    void DecreaseVol()
    {
        if (music.volume > 0f)
        {
            music.volume -= 0.05f;
            Preferences.CurrentVolume = music.volume;
        }
    }
}
