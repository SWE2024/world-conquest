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

    public static int playerCount = 2; // 2 is the default amount of players

    // Start is called before the first frame update
    void Start()
    {
        btnReady.onClick.AddListener(LoadGame);
        btnLeave.onClick.AddListener(LoadMenu);
        btnMinus.onClick.AddListener(HandleMinus);
        btnPlus.onClick.AddListener(HandlePlus);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void HandleMinus()
    {
        if (playerCount <= 2) { return; }
        playerCount--;
        number.text = $"{playerCount}";
    }

    void HandlePlus()
    {
        if (playerCount >= 6) { return; }
        playerCount++;
        number.text = $"{playerCount}";
    }

    void LoadGame()
    {
        Map1.playerCount = playerCount;
        SceneManager.LoadScene("assets/scenes/scenegame.unity");
    }

    void LoadMenu()
    {
        playerCount = 2;
        SceneManager.LoadScene("assets/scenes/scenemainmenu.unity");
    }
}
