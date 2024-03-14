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

    void Map1() { Settings.MapNumber = 1; }

    void Map2() { Settings.MapNumber = 2; }

    void HandleMinus()
    {
        if (Settings.PlayerCount <= 2) { return; }
        Settings.PlayerCount--;
        number.text = $"{Settings.PlayerCount}";
    }

    void HandlePlus()
    {
        if (Settings.PlayerCount >= 6) { return; }
        Settings.PlayerCount++;
        number.text = $"{Settings.PlayerCount}";
    }

    void LoadGame()
    {
        Wait.Start(1.5f, () =>
        {
            SceneManager.LoadScene("assets/scenes/scenegame.unity");
        });
    }

    void LoadMenu()
    {
        Settings.PlayerCount = 2;
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
