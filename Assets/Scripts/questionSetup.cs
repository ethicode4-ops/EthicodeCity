using UnityEngine;

public class questionSetup : MonoBehaviour
{

  public questionManager questionManager;

    public Sprite Question1;
    public Sprite Question2;
    public Sprite Question3;
    public Sprite Question4;
    public Sprite Question5;
    public Sprite Question6;
    public Sprite Question7;
    public Sprite Question8;
    public Sprite Question9;
    public Sprite Question10;

    void Start()
    {
        questionManager.questionImages = new Sprite[] { Question1, Question2,Question3, Question4, Question5, Question6, Question7, Question8, Question9, Question10 };
    }

}
