using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ScriptAudio : MonoBehaviour
{
    [SerializeField] AudioSource music;
    [SerializeField] Button volUp;
    [SerializeField] Button volDown;

    public static ScriptAudio menuMusic;

    float currentVolume = 0.15f;

    void Awake()
    {
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("GameMusic");
        if (musicObj.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        music.volume = currentVolume;

        volUp.onClick.AddListener(IncreaseVol);
        volDown.onClick.AddListener(DecreaseVol);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (music.volume == 0f) music.volume = currentVolume;
            else music.volume = 0f;
        }

        if (SceneManager.GetActiveScene().name == "SceneGame")
        {
            music.Stop();
        }
    }

    void IncreaseVol()
    {
        if (music.volume < 0.5f)
        {
            music.volume += 0.05f;
            currentVolume = music.volume;
        }
    }

    void DecreaseVol()
    {
        if (music.volume > 0f)
        {
            music.volume -= 0.05f;
            currentVolume = music.volume;
        }
    }
}
