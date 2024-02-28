using UnityEngine;
using UnityEngine.UI;

public class ScriptButtonHover : MonoBehaviour
{
    [SerializeField] Image img;

    // Start is called before the first frame update
    void Start()
    {
        img.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter()
    {
        img.enabled = true;
    }

    public void OnPointerLeave()
    {
        img.enabled = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
