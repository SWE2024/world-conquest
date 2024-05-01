using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickMainMenu : MonoBehaviour
{
    [SerializeField] Button btnPlay;
    [SerializeField] Button btnExit;
    [SerializeField] Image trianglePlay;
    [SerializeField] Image triangleExit;

    // Start is called before the first frame update
    void Start()
    {
        trianglePlay.enabled = false;
        triangleExit.enabled = false;

        btnPlay.onClick.AddListener(LoadLobby);
        btnExit.onClick.AddListener(ExitGame);
    }

    /// <summary>
    /// <c>LoadLobby</c> resets the preferences of the game and opens the lobby scene.
    /// </summary>
    void LoadLobby()
    {
        // reset preferences to default values in case they have been changed
        Preferences.PlayerCount = 2;
        Preferences.AgentCount = 1;
        Preferences.MapNumber = 1;
        Preferences.AutoPopulateFlag = false;

        // load the game scene
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(0.75f);
        SceneManager.LoadScene("assets/scenes/scenelobby.unity");
    }

    /// <summary>
    /// <c>ExitGame</c> completely closes the game.
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

    public void PointerEnterPlay() { trianglePlay.enabled = true; }

    public void PointerEnterExit() { triangleExit.enabled = true; }

    public void PointerLeavePlay() { trianglePlay.enabled = false; }

    public void PointerLeaveExit() { triangleExit.enabled = false; }
}
