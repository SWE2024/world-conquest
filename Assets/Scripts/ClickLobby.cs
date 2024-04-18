using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour
{
    [SerializeField] Button btnMap1;
    [SerializeField] Button btnMap2;
    [SerializeField] Button btnReady;
    [SerializeField] Button btnLeave;
    [SerializeField] Button btnMinus;
    [SerializeField] Button btnPlus;
    [SerializeField] TextMeshProUGUI number;

    [SerializeField] Image triangleReady;
    [SerializeField] Image triangleLeave;

    [SerializeField] AudioClip gameMusic;

    // Start is called before the first frame update
    void Start()
    {
        triangleReady.enabled = false;
        triangleReady.enabled = false;

        btnMap1.onClick.AddListener(Map1);
        btnMap2.onClick.AddListener(Map2);
        btnReady.onClick.AddListener(LoadGame);
        btnLeave.onClick.AddListener(LoadMenu);
        btnMinus.onClick.AddListener(HandleMinus);
        btnPlus.onClick.AddListener(HandlePlus);
    }

    void Map1() { Preferences.MapNumber = 1; }

    void Map2() { Preferences.MapNumber = 2; }

    void HandleMinus()
    {
        if (Preferences.PlayerCount <= 2) { return; }
        Preferences.PlayerCount--;
        number.text = $"{Preferences.PlayerCount}";
    }

    void HandlePlus()
    {
        if (Preferences.PlayerCount >= 6) { return; }
        Preferences.PlayerCount++;
        number.text = $"{Preferences.PlayerCount}";
    }

    void LoadGame()
    {
        Wait.Start(1.5f, () =>
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
        Wait.Start(1.5f, () =>
        {
            SceneManager.LoadScene("assets/scenes/scenemainmenu.unity");
        });
    }

    public void PointerEnterReady() { triangleReady.enabled = true; }

    public void PointerEnterLeave() { triangleLeave.enabled = true; }

    public void PointerLeaveReady() { triangleReady.enabled = false; }

    public void PointerLeaveLeave() { triangleLeave.enabled = false; }
}
