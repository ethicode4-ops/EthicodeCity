using UnityEngine;

public class CameraBaseController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (transform.GetComponentInChildren<CameraManager>().isDragging)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.localRotation *= Quaternion.Euler(0, 1, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.localRotation *= Quaternion.Euler(0, -1, 0);
            }
        }
    }
}
