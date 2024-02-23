using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptStartGame : MonoBehaviour
{
    private AsyncOperation asyncOperation;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadGameScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleClick()
    {
        asyncOperation.allowSceneActivation = true;
    }

    IEnumerator LoadGameScene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Assets/Scenes/Game.unity");
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Progress: " + asyncOperation.progress);

        yield return null;
    }
}
