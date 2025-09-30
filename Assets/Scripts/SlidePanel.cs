using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class SlidePanelController : MonoBehaviour
{
    public GameObject panel;  // Reference to the panel to slide in/out
    public float slideSpeed = 5f;  // Speed of the sliding animation
    private bool isPanelVisible = false;  // Track the state of the panel

    private Vector3 hiddenPosition;  // Position when panel is hidden (offscreen to the right)
    private Vector3 visiblePosition;  // Position when panel is fully visible (just covering its width)

    void Start()
    {
        // Get the initial position of the panel where it is placed in the scene
        hiddenPosition = panel.GetComponent<RectTransform>().anchoredPosition;

        // Calculate the final visible position based on the panel's width (it will stop after the width of the panel)
        visiblePosition = new Vector3(hiddenPosition.x - panel.GetComponent<RectTransform>().rect.width, 0, 0);

        // Set the panel to the hidden position initially (off-screen)
        panel.GetComponent<RectTransform>().anchoredPosition = hiddenPosition;
    }

    public void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;  // Toggle panel visibility

        // Start the sliding animation
        StopAllCoroutines();
        StartCoroutine(SlidePanel(isPanelVisible ? visiblePosition : hiddenPosition));
    }

    private System.Collections.IEnumerator SlidePanel(Vector3 targetPosition)
    {
        Vector3 currentPosition = panel.GetComponent<RectTransform>().anchoredPosition;

        // Slide the panel towards the target position (from right to left)
        while (Vector3.Distance(currentPosition, targetPosition) > 0.01f)
        {
            currentPosition = Vector3.Lerp(currentPosition, targetPosition, slideSpeed * Time.deltaTime);
            panel.GetComponent<RectTransform>().anchoredPosition = currentPosition;
            yield return null;
        }

        // Ensure panel is exactly at the target position
        panel.GetComponent<RectTransform>().anchoredPosition = targetPosition;
    }
    public void EnableAchievement(string Name)
    {
        // panel.transform.Find(Name).gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 1);
        panel.transform.Find(Name).gameObject.transform.Find("Text (TMP)").GameObject().SetActive(true);
    }
}
