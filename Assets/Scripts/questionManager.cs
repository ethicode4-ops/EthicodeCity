using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class questionManager : MonoBehaviour
{
    public Image questionImage;           // UI Image to display the question
    public TMP_InputField inputField;     // Input field for user answers
    public Button submitButton;           // Submit button
    public Sprite[] questionImages;       // Array of question images

    private int currentQuestionIndex = 0;

    public IEnumerator StartQuestion()
    {
        transform.parent.gameObject.SetActive(true);
        DisplayQuestion();
        submitButton.onClick.AddListener(HandleSubmit);
        while (currentQuestionIndex < questionImages.Length)
        {
            yield return false;
        }
        yield return true;
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex < questionImages.Length)
        {
            questionImage.sprite = questionImages[currentQuestionIndex]; // Update the image
            questionImage.enabled = true;  // Make sure the image is visible
            inputField.text = "";          // Clear the input field
        }
        else
        {
            EndQuiz();
        }
    }

    void HandleSubmit()
    {
        string userAnswer = inputField.text;
        if (!string.IsNullOrWhiteSpace(userAnswer))
        {
            Debug.Log($"Answer for Question {currentQuestionIndex + 1}: {userAnswer}");
            currentQuestionIndex++;
            DisplayQuestion();
        }
        else
        {
            Debug.Log("Please enter an answer before submitting.");
        }
    }
    void EndQuiz()
    {
        Debug.Log("Quiz Completed!");
        questionImage.gameObject.SetActive(false);
        inputField.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(false);
    }

}
