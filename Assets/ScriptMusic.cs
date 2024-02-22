using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    [SerializeField] AudioSource musicMenu;
    [SerializeField] bool isMuted;

    // Start is called before the first frame update
    void Start()
    {
        musicMenu.enabled = true;
        musicMenu.mute = isMuted;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isMuted = !isMuted;
            musicMenu.mute = isMuted;
        }
    }
}
