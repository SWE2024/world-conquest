using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Win : MonoBehaviour
{
    [SerializeField] AudioClip winMusic;
    [SerializeField] AudioClip lobbyMusic;
    [SerializeField] Button btnLeave;
    AudioSource music;

    // Start is called before the first frame update
    void Start()
    {
        music = GameObject.Find("Music").GetComponent<AudioSource>(); // switches music based on scene
        music.clip = winMusic;
        music.Play();

        btnLeave.onClick.AddListener(LoadMenu);

        // handle moving data here

        /*
         * for(int i = 0; i < game.eliminatedPlayers.Count; i++) ranking += $"{places[i]}: {game.eliminatedPlayers[i].GetName()}\n";
         * GameObject.Find("EliminatedList").GetComponent<TextMeshProUGUI>().text = ranking;
         * GameObject.Find("WinnerUsername").GetComponent<TextMeshProUGUI>().text = game.turnPlayer.GetName();
         * GameObject.Find("WinnerColour").GetComponent<Image>().color = game.turnPlayer.GetColor();
         */
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
}
