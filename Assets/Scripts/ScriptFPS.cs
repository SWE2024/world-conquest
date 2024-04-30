using TMPro;
using UnityEngine;

/// <summary>
/// <c>ScriptFPS</c> controls the screen FPS counter and the display refresh rate.
/// </summary>
public class ScriptFPS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;
    float fps;
    float updateTimer = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value; // limits frame rate to monitor refresh rate
        fpsText.enabled = Preferences.isShownFPS;
    }

    // Update is called once per frame
    void Update()
    {
        toggleFpsCounter();
        toggleFullscreen();
        if (Preferences.isShownFPS) updateFpsCounter(); // updates fps counter every frame
    }

    /// <summary>
    /// <c>toggleFpsCounter</c> show / hides the FPS counter.
    /// </summary>
    private void toggleFpsCounter()
    {
        if (Input.GetKeyDown(KeyCode.V) && !GameObject.Find("RenameCountry").GetComponent<Canvas>().enabled)
        {
            if (Preferences.isShownFPS) fpsText.enabled = false;
            else fpsText.enabled = true;

            Preferences.isShownFPS = !Preferences.isShownFPS; // disables frame rate (or enables)
        }
    }

    /// <summary>
    /// <c>toggleFpsCounter</c> toggles fullscreen mode (window is resizable).
    /// </summary>
    private void toggleFullscreen()
    {
        if (Input.GetKeyDown(KeyCode.F) && !GameObject.Find("RenameCountry").GetComponent<Canvas>().enabled)
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    /// <summary>
    /// <c>updateFpsCounter</c> changes the number displayed in the FPS counter.
    /// </summary>
    private void updateFpsCounter()
    {
        updateTimer -= Time.deltaTime; // take away time since last frame
        if (updateTimer <= 0f)
        {
            fps = 1f / Time.unscaledDeltaTime;
            fpsText.text = "FPS: " + Mathf.Round(fps);
            updateTimer = 0.2f;
        }
    }
}
