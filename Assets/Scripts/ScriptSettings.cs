using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptSettings : MonoBehaviour
{
    [SerializeField] Button btnSettingsEnter;
    [SerializeField] Button btnSettingsLeave;
    [SerializeField] Canvas buttons;

    [SerializeField] Button btnVolUp;
    [SerializeField] Button btnVolDown;
    [SerializeField] Button btnLeave;
    [SerializeField] Button btnExit;

    [SerializeField] AudioClip clipMenu;
    [SerializeField] AudioClip clipGame;

    // Start is called before the first frame update
    void Start()
    {
        buttons.enabled = false;

        btnSettingsEnter.onClick.AddListener(OpenSettingsMenu);
        btnSettingsLeave.onClick.AddListener(CloseSettingsMenu);

        btnVolUp.onClick.AddListener(IncreaseVol);
        btnVolDown.onClick.AddListener(DecreaseVol);
        btnLeave.onClick.AddListener(ReturnToLobby);
        btnExit.onClick.AddListener(ExitGame);

        switch (SceneManager.GetActiveScene().name)
        {
            case "SceneMainMenu":
                btnLeave.enabled = false;
                break;
            case "SceneLobby":
                btnLeave.enabled = false;
                break;
            default:
                break;
        }
    }

    void OpenSettingsMenu()
    {
        buttons.enabled = true;

        if (SceneManager.GetActiveScene().name == "SceneGame")
        {
            GameObject.Find("CanvasUI").GetComponent<Canvas>().enabled = false;
        }
    }

    void CloseSettingsMenu()
    {
        buttons.enabled = false;

        if (SceneManager.GetActiveScene().name == "SceneGame")
        {
            GameObject.Find("CanvasUI").GetComponent<Canvas>().enabled = true;
        }
    }

    void IncreaseVol()
    {
        ScriptAudio.IncreaseVol();
    }

    void DecreaseVol()
    {
        ScriptAudio.DecreaseVol();
    }

    void ReturnToLobby()
    {
        Wait.Start(0.75f, () =>
        {
            SceneManager.LoadScene("assets/scenes/scenemainmenu.unity");
            AudioSource music = GameObject.Find("Music").GetComponent<AudioSource>(); // switches music based on scene
            music.clip = clipMenu;
            music.Play();
        });
    }

    void ExitGame()
    {
        StartCoroutine(CloseGame());
    }

    private IEnumerator CloseGame()
    {
        yield return new WaitForSeconds(0.75f);
        Application.Quit();
    }
}
