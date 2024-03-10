using TMPro;
using UnityEngine;

public class FPSUpdate : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;
    [SerializeField] bool isShown = true;
    float fps;
    float updateTimer = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 240; // limits frame rate to 240
        fpsText.enabled = isShown;
    }

    // Update is called once per frame
    void Update()
    {
        toggleFpsCounter();
        if (isShown) updateFpsCounter(); // updates fps counter every frame
    }

    private void toggleFpsCounter()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isShown) fpsText.enabled = false;
            else fpsText.enabled = true;

            isShown = !isShown; // disables frame rate (or enables)
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
