using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Win : MonoBehaviour
{
    [SerializeField] AudioClip winMusic;
    [SerializeField] AudioClip lobbyMusic;
    [SerializeField] Button btnLeave;
    AudioSource music;

    public static string eliminatedList = "";
    public static Color winnerColor;
    public static string username = "";

    // Start is called before the first frame update
    void Start()
    {
        music = GameObject.Find("Music").GetComponent<AudioSource>(); // switches music based on scene
        music.clip = winMusic;
        music.Play();        
        GameObject.Find("EliminatedList").GetComponent<TextMeshProUGUI>().text = eliminatedList;
        GameObject.Find("WinnerUsername").GetComponent<TextMeshProUGUI>().text = username;
        GameObject.Find("WinnerColour").GetComponent<Image>().color = winnerColor;
        
    }

    void LoadMenu()
    {
        Preferences.PlayerCount = 2;
        Preferences.AgentCount = 1;
        Preferences.MapNumber = 1;
        Wait.Start(0.75f, () =>
        {
            SceneManager.LoadScene("assets/scenes/scenemainmenu.unity");
            music.clip = lobbyMusic;
            music.Play();
        });
    }


    void Update() 
    {

    }
}
