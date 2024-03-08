using UnityEngine;
using UnityEngine.UI;

public class ScriptSettings : MonoBehaviour
{
    [SerializeField] Button btnSettingsEnter;

    // Start is called before the first frame update
    void Start()
    {
        Button btnSettingsLeave = GameObject.Find("ButtonSettingsLeave").GetComponent<Button>();
        btnSettingsEnter.onClick.AddListener(OpenSettingsMenu);
        btnSettingsLeave.onClick.AddListener(CloseSettingsMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenSettingsMenu()
    {
        GameObject.Find("CanvasSettings").GetComponent<Canvas>().enabled = true;
    }

    void CloseSettingsMenu()
    {
        GameObject.Find("CanvasSettings").GetComponent<Canvas>().enabled = false;
    }
}
