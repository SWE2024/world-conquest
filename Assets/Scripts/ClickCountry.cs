using UnityEngine;
using UnityEngine.UI;

public class ClickCountry : MonoBehaviour
{
    [SerializeField] Image btn;

    // Start is called before the first frame update
    void Start()
    {
        btn.alphaHitTestMinimumThreshold = 0.5f;
    }
}
