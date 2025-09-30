using UnityEngine;

public class EarthquakeMovement : MonoBehaviour
{
    public float shakeDuration = 1.0f;
    public float shakeMagnitude = 0.1f;
    private Vector3 originalPosition;
    private float elapsed = 0.0f;
    private bool isEarthquakeActive = false;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (isEarthquakeActive)
        {
            if (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float percentComplete = elapsed / shakeDuration;
                float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
                float x = (Random.value * 2.0f - 1.0f) * shakeMagnitude * damper;
                float y = (Random.value * 2.0f - 1.0f) * shakeMagnitude * damper;
                transform.localPosition = originalPosition + new Vector3(x, y, 0);
            }
            else
            {
                elapsed = 0.0f;
                isEarthquakeActive = false;
                transform.localPosition = originalPosition;
            }
        }
    }
    public void TriggerEarthquake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        elapsed = 0.0f;
        isEarthquakeActive = true;
    }
}
