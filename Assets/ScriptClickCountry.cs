using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptClickCountry : MonoBehaviour
{
    [SerializeField] Image btn;

    // Start is called before the first frame update
    void Start()
    {
        btn.alphaHitTestMinimumThreshold = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
