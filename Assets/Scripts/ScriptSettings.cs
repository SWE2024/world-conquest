using UnityEngine;
using UnityEngine.UI;

public class ScriptSettings : MonoBehaviour
{
    [SerializeField] Button btnSettingsEnter;
    [SerializeField] Button btnSettingsLeave;
    [SerializeField] Canvas buttons;

    // Start is called before the first frame update
    void Start()
    {
        buttons.enabled = false;
        btnSettingsEnter.onClick.AddListener(OpenSettingsMenu);
        btnSettingsLeave.onClick.AddListener(CloseSettingsMenu);
    }

    void OpenSettingsMenu()
    {
        buttons.enabled = true;
    }

    void CloseSettingsMenu()
    {
        buttons.enabled = false;
    }
}
