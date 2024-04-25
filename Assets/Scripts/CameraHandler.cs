using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// <c>CameraHandler</c> controls all movement of the camera, including panning and zooming.
/// </summary>
public class CameraHandler : MonoBehaviour
{
    public static bool DisableMovement = false;

    [SerializeField] Camera cam;
    [SerializeField] RectTransform zone;
    Vector3 dragOrigin;
    float zoomStep, minCamSize, maxCamSize, velocity, zoom;

    // Start is called before the first frame update
    void Start()
    {
        // change accordingly if the canvas ever changes size
        // higher size means less zoom, etc etc.
        minCamSize = 100;
        maxCamSize = 800;
        zoomStep = 200;
        zoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (DisableMovement) return;
        if (Input.mousePosition.y < 305) return; // prevents movement in killfeed

        PanCamera();
        ZoomCamera();
    }

    /// <summary>
    /// <c>PanCamera</c> moves the camera when the user drags the mouse.
    /// </summary>
    private void PanCamera()
    {
        if (cam.orthographicSize < maxCamSize - 10) // prevents moving out further than the game board
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition); // get the location of the first click
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
                float newLocationX = Mathf.Clamp(cam.transform.position.x + (difference.x), 890, 1670);
                float newLocationY = Mathf.Clamp(cam.transform.position.y + (difference.y), 320, 940);
                // transform the camera by the original position to the new location
                cam.transform.position = new Vector3(newLocationX, newLocationY, -10);
            }
        }
    }

    /// <summary>
    /// <c>ZoomCamera</c> zooms the camera with the mouse scroll wheel.
    /// </summary>
    private void ZoomCamera()
    {
        if (cam.orthographicSize >= maxCamSize - 10) // prevents zooming out further than intended
        {
            Vector3 reset = Vector3.Lerp(cam.transform.position, new Vector3(1280, 720, -10), 5f * Time.deltaTime);
            cam.transform.position = reset; // smooths camera back to default location
        }

        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomStep; // zoom out by the zoomstep and amount of scrolls
        zoom = Mathf.Clamp(zoom, minCamSize, maxCamSize);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, 0.1f);
    }
}
