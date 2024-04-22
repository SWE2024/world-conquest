using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// <c>ScriptSettings</c> handles the in-game settings menu.
/// </summary>
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

    bool isOpen = false;

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isOpen) OpenSettingsMenu();
            else CloseSettingsMenu();

            isOpen = !isOpen;
        }
    }

    /// <summary>
    /// <c>OpenSettingsMenu</c> enables the settings menu canvas to show the buttons.
    /// </summary>
    void OpenSettingsMenu()
    {
        buttons.enabled = true;

        if (SceneManager.GetActiveScene().name == "SceneGame")
        {
            GameObject.Find("CanvasUI").GetComponent<Canvas>().enabled = false;
        }
    }

    /// <summary>
    /// <c>CloseSettingsMenu</c> disables the settings menu canvas to show the game.
    /// </summary>
    void CloseSettingsMenu()
    {
        buttons.enabled = false;

        if (SceneManager.GetActiveScene().name == "SceneGame")
        {
            GameObject.Find("CanvasUI").GetComponent<Canvas>().enabled = true;
        }
    }

    /// <summary>
    /// <c>IncreaseVol</c> calls IncreaseVol.
    /// </summary>
    void IncreaseVol()
    {
        ScriptAudio.IncreaseVol();
    }

    /// <summary>
    /// <c>DecreaseVol</c> calls DecreaseVol.
    /// </summary>
    void DecreaseVol()
    {
        ScriptAudio.DecreaseVol();
    }

    /// <summary>
    /// <c>ReturnToLobby</c> closes / ends your game and reloads the main menu scene.
    /// </summary>
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

    /// <summary>
    /// <c>ExitGame</c> closes the entire application.
    /// </summary>
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
