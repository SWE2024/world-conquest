using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour
{
    [SerializeField] Button btnMap1;
    [SerializeField] Button btnMap2;
    [SerializeField] Button btnMap3;

    [SerializeField] Button btnReady;
    [SerializeField] Button btnLeave;

    [SerializeField] Button btnMinusPlayer;
    [SerializeField] Button btnPlusPlayer;
    [SerializeField] TextMeshProUGUI numberPlayers;

    [SerializeField] Button btnMinusAgent;
    [SerializeField] Button btnPlusAgent;
    [SerializeField] TextMeshProUGUI numberAgents;

    [SerializeField] Image triangleReady;
    [SerializeField] Image triangleLeave;

    [SerializeField] AudioClip gameMusic;

    // Start is called before the first frame update
    void Start()
    {
        triangleReady.enabled = false;
        triangleLeave.enabled = false;

        btnMap1.onClick.AddListener(Map1);
        btnMap2.onClick.AddListener(Map2);
        btnMap3.onClick.AddListener(Map3);
        btnMap1.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        btnMap2.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        btnMap3.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

        btnReady.onClick.AddListener(LoadGame);
        btnLeave.onClick.AddListener(LoadMenu);

        btnMinusPlayer.onClick.AddListener(HandleMinusPlayer);
        btnPlusPlayer.onClick.AddListener(HandlePlusPlayer);

        btnMinusAgent.onClick.AddListener(HandleMinusAgent);
        btnPlusAgent.onClick.AddListener(HandlePlusAgent);
    }

    void Map1() { Preferences.MapNumber = 1; }

    void Map2() { Preferences.MapNumber = 2; }

    void Map3() { Preferences.MapNumber = 3; }

    void HandleMinusPlayer()
    {
        if ((Preferences.PlayerCount <= 2 && Preferences.AgentCount == 0) || (Preferences.PlayerCount == 1)) { return; }
        Preferences.PlayerCount--;
        numberPlayers.text = $"{Preferences.PlayerCount}";
    }

    void HandlePlusPlayer()
    {
        if (Preferences.PlayerCount + Preferences.AgentCount >= 6) { return; }
        Preferences.PlayerCount++;
        numberPlayers.text = $"{Preferences.PlayerCount}";
    }

    void HandleMinusAgent()
    {
        if (Preferences.AgentCount + Preferences.PlayerCount <= 2) { return; }
        Preferences.AgentCount--;
        numberAgents.text = $"{Preferences.AgentCount}";
    }

    void HandlePlusAgent()
    {
        if (Preferences.AgentCount + Preferences.PlayerCount >= 6) { return; }
        Preferences.AgentCount++;
        numberAgents.text = $"{Preferences.AgentCount}";
    }

    void LoadGame()
    {
        Wait.Start(0.75f, () =>
        {
            SceneManager.LoadScene("assets/scenes/scenegame.unity");
            AudioSource music = GameObject.Find("Music").GetComponent<AudioSource>(); // switches music based on scene
            music.clip = gameMusic;
            music.Play();
        });
    }

    void LoadMenu()
    {
        Preferences.PlayerCount = 2;
        Preferences.AgentCount = 1;
        Preferences.MapNumber = 1;
        Wait.Start(0.75f, () =>
        {
            SceneManager.LoadScene("assets/scenes/scenemainmenu.unity");
        });
    }

    public void PointerEnterReady() { triangleReady.enabled = true; }

    public void PointerEnterLeave() { triangleLeave.enabled = true; }

    public void PointerLeaveReady() { triangleReady.enabled = false; }

    public void PointerLeaveLeave() { triangleLeave.enabled = false; }
}
