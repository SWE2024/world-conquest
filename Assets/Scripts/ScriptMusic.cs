using UnityEngine;

public class MusicScript : MonoBehaviour
{
    [SerializeField] AudioSource musicTrack;
    private bool isMuted;

    // Start is called before the first frame update
    void Start()
    {
        musicTrack.enabled = true;
        musicTrack.mute = isMuted;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isMuted = !isMuted;
            musicTrack.mute = isMuted;
        }
    }
}
