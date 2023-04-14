using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;

public class SpellingController : MonoBehaviour
{
    public Text QuestionPrompt;
    public Text CurrentAnswerDisplay;
    public ButtonActivated[] LetterButtons;
    public Text[] LetterPrompts;
    public UnityEvent CorrectAction;
    public MMFeedbacks CorrectFeedback;

    private string[] SpellingQuestionBank = { "What animal meows?", "What animal moos?", "What animal barks?", "What animal hoots?" };
    private string[] SpellingAnswerBank = { "C A T", "C O W", "D O G", "O W L" };
    private string[] AlternateLetters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    private string CurrentAnswer = "_ _ _";
    private int CurrentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        int questionNumber = Random.Range(0, SpellingQuestionBank.Length);
        string QuestionString = SpellingQuestionBank[questionNumber];
        string AnswerString = SpellingAnswerBank[questionNumber];
        QuestionPrompt.text = QuestionString;
        CurrentAnswerDisplay.text = CurrentAnswer;

        string[] answerLetters = AnswerString.Split(" ");
        Debug.Log(QuestionString);
        Debug.Log(answerLetters);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
