using UnityEngine;
using SpeechLib;

public class SpeechManager : MonoBehaviour
{
    private SpVoice voice;
    void Start()
    {
        voice = new SpVoice();
        if (voice == null)
        {
            Debug.LogError("Failed to create SpVoice instance.");
        }
    }

    public void SpeakLine(string str)
    {
        voice.Speak(str);
    }
}
