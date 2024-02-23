using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptButtonClick : MonoBehaviour
{
    [SerializeField] Button btnPlay;
    [SerializeField] Button btnExit;

    // Start is called before the first frame update
    void Start()
    {
        btnPlay.onClick.AddListener(LoadGame);
        btnExit.onClick.AddListener(ExitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadGame()
    {
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("assets/scenes/scenegame.unity");
    }

    void ExitGame()
    {
        StartCoroutine(CloseGame());
    }

    private IEnumerator CloseGame()
    {
        yield return new WaitForSeconds(1);
        Application.Quit();
    }
}
