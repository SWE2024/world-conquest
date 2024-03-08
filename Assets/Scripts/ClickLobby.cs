using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour
{
    [SerializeField] Button btnReady;
    [SerializeField] Button btnLeave;
    [SerializeField] Button btnMinus;
    [SerializeField] Button btnPlus;
    [SerializeField] TextMeshProUGUI number;

    public static int PlayerCount = 2; // 2 is the default amount of players

    // Start is called before the first frame update
    void Start()
    {
        btnReady.onClick.AddListener(LoadGame);
        btnLeave.onClick.AddListener(LoadMenu);
        btnMinus.onClick.AddListener(HandleMinus);
        btnPlus.onClick.AddListener(HandlePlus);
    }

    void HandleMinus()
    {
        if (PlayerCount <= 2) { return; }
        PlayerCount--;
        number.text = $"{PlayerCount}";
    }

    void HandlePlus()
    {
        if (PlayerCount >= 6) { return; }
        PlayerCount++;
        number.text = $"{PlayerCount}";
    }

    void LoadGame()
    {
        Map1.PlayerCount = PlayerCount;
        Wait.Start(1.5f, () =>
        {
            SceneManager.LoadScene("assets/scenes/scenegame.unity");
        });
    }

    void LoadMenu()
    {
        PlayerCount = 2;
        Wait.Start(1.5f, () =>
        {
            SceneManager.LoadScene("assets/scenes/scenemainmenu.unity");
        });
    }
}
