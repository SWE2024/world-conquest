using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSUpdate : MonoBehaviour
{
    float fps;
    float updateTimer = 0.25f;
    [SerializeField] TextMeshProUGUI fpsText;
    [SerializeField] bool isShown = true;

    // Start is called before the first frame update
    void Start()
    {
        fpsText.enabled = isShown;
    }

    // Update is called once per frame
    void Update()
    {
        toggleFpsCounter();
        if (isShown)
        {
            updateFpsCounter();
        }
    }

    private void toggleFpsCounter()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isShown)
            {
                fpsText.enabled = false;
            }
            else
            {
                fpsText.enabled = true;
            }

            isShown = !isShown;
        }
    }

    private void updateFpsCounter()
    {
        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f)
        {
            fps = 1f / Time.unscaledDeltaTime;
            fpsText.text = "FPS: " + Mathf.Round(fps);
            updateTimer = 0.25f;
        }
    }
}
