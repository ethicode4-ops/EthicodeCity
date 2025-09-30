using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    private float dragSpeed = 2f; // Speed of the camera movement
    public float dragRatio = 5; // Ratio of the camera movement speed to the orthographic size
    private Vector3 dragOrigin; // Stores the initial mouse position
    public bool isDragging = false;

    public GameObject parent;

    public Button[] disableButtons;

    public Button[] enableButtons;

    [SerializeField]
    public bool cameraDragEnabled = true;

    void Start()
    {
        for (int i = 0; i < disableButtons.Length; i++)
        {
            disableButtons[i].onClick.AddListener(() => cameraDragEnabled = false);
        }
        for (int i = 0; i < enableButtons.Length; i++)
        {
            enableButtons[i].onClick.AddListener(() => cameraDragEnabled = true);
        }
    }
    void Update()
    {
        HandleCameraDrag();
        HandleZoom();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && cameraDragEnabled)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * 2, 2, 7);
            dragSpeed = Camera.main.orthographicSize / dragRatio;
        }
    }
    void HandleCameraDrag()
    {
        if (Input.GetMouseButtonDown(0) && cameraDragEnabled) // Left mouse button pressed
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            isDragging = false;
        }

        if (isDragging && cameraDragEnabled)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 difference = dragOrigin - currentMousePosition;

            // Convert the difference to world space considering the camera's orientation
            Vector3 move = new Vector3(difference.x, 0, difference.y) * dragSpeed * Time.deltaTime;
            move = parent.transform.TransformDirection(move);

            // Calculate the new position
            Vector3 newPosition = parent.transform.position + move;

            // Clamp the new position within the desired range
            newPosition.x = Mathf.Clamp(newPosition.x, 7f, 24f);
            newPosition.z = Mathf.Clamp(newPosition.z, 7f, 24f);

            // Apply the clamped position
            parent.transform.position = newPosition;

            // Update drag origin
            dragOrigin = currentMousePosition;
        }
    }
}
