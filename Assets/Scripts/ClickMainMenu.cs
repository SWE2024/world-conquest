using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickMainMenu : MonoBehaviour
{
    [SerializeField] Button btnPlay;
    [SerializeField] Button btnExit;
    [SerializeField] Button btnSettingsEnter;
    [SerializeField] Button btnSettingsLeave;
    [SerializeField] Canvas menuSettings;

    // Start is called before the first frame update
    void Start()
    {
        menuSettings.enabled = false;
        btnPlay.onClick.AddListener(LoadLobby);
        btnExit.onClick.AddListener(ExitGame);
        btnSettingsEnter.onClick.AddListener(SettingsEnter);
        btnSettingsLeave.onClick.AddListener(SettingsLeave);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadLobby()
    {
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("assets/scenes/scenelobby.unity");
    }

    void SettingsEnter()
    {
        menuSettings.enabled = true;
    }

    void SettingsLeave()
    {
        menuSettings.enabled = false;
    }

    void ExitGame()
    {
        StartCoroutine(CloseGame());
    }

    private IEnumerator CloseGame()
    {
        yield return new WaitForSeconds(1);
        Application.Quit();
    }
}
