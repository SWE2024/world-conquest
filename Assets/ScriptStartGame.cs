using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptStartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleClick()
    {
        // sound for button click is not loading here
        SceneManager.LoadScene("Assets/Scenes/Game.unity");
        // StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("Assets/Scenes/Game.unity");
        while (!load.isDone)
        {
            yield return null;
        }
    }
}
