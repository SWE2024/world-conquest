using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour
{
    [SerializeField] Button btnReady;
    [SerializeField] Button btnLeave;

    // Start is called before the first frame update
    void Start()
    {
        btnReady.onClick.AddListener(LoadGame);
        btnLeave.onClick.AddListener(LoadMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadGame()
    {
        StartCoroutine(SwitchToGame());
    }

    void LoadMenu()
    {
        StartCoroutine(SwitchToMenu());
    }

    private IEnumerator SwitchToGame()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("assets/scenes/scenegame.unity");
    }

    private IEnumerator SwitchToMenu()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("assets/scenes/scenemainmenu.unity");
    }
}
