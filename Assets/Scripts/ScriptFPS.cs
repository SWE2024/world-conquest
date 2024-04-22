using TMPro;
using UnityEngine;


public class FPSUpdate : MonoBehaviour
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

    private void toggleFpsCounter()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (Preferences.isShownFPS) fpsText.enabled = false;
            else fpsText.enabled = true;

            Preferences.isShownFPS = !Preferences.isShownFPS; // disables frame rate (or enables)
        }
    }

    private void toggleFullscreen()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!Screen.fullScreen)
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,
                    FullScreenMode.ExclusiveFullScreen, Screen.currentResolution.refreshRateRatio);
            }
            else
            {
                Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2,
                    FullScreenMode.Windowed, Screen.currentResolution.refreshRateRatio);
            }
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

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
