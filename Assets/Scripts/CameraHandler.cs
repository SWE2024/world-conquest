using Microsoft.Win32.SafeHandles;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraHandler : MonoBehaviour
{
    public static bool DisableMovement = false;

    [SerializeField] Camera cam;
    Vector3 dragOrigin;
    float zoomStep, minCamSize, maxCamSize, velocity, zoom;

    // Start is called before the first frame update
    void Start()
    {
        // change accordingly if the canvas ever changes size
        // higher size means less zoom, etc etc.
        minCamSize = 150;
        maxCamSize = 800;
        zoomStep = 150;
        zoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (DisableMovement) return;

        PanCamera();
        ZoomCamera();
    }

    private void PanCamera()
    {
        if (cam.orthographicSize < maxCamSize) // prevents moving out further than the game
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
                float newLocationX = Mathf.Clamp(cam.transform.position.x + (difference.x * 0.33f), 890, 1670);
                float newLocationY = Mathf.Clamp(cam.transform.position.y + (difference.y * 0.33f), 320, 940);
                cam.transform.position = new Vector3(newLocationX, newLocationY, -10);
            }
        }
        else
        {
            cam.transform.position = new Vector3(1280, 720, -10); // sets camera back to default location when fully zoomed out
        }
    }

    private void ZoomCamera()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomStep;
        zoom = Mathf.Clamp(zoom, minCamSize, maxCamSize);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, 0.1f);
    }
}
