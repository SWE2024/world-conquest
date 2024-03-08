using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickMainMenu : MonoBehaviour
{
    [SerializeField] Button btnPlay;
    [SerializeField] Button btnExit;
    [SerializeField] Canvas menuSettings;

    // Start is called before the first frame update
    void Start()
    {
        menuSettings.enabled = false;
        btnPlay.onClick.AddListener(LoadLobby);
        btnExit.onClick.AddListener(ExitGame);
    }

    void LoadLobby()
    {
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("assets/scenes/scenelobby.unity");
    }

    void ExitGame()
    {
        StartCoroutine(CloseGame());
    }

    private IEnumerator CloseGame()
    {
        yield return new WaitForSeconds(1.5f);
        Application.Quit();
    }
}
