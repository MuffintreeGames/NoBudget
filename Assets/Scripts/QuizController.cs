using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;

public class QuizController : MonoBehaviour
{
    public int QuestionDifficulty = 0;
    public Text QuestionPrompt;
    public ButtonActivated[] AnswerButtons;
    public Text[] AnswerPrompts;
    public UnityEvent CorrectAction;
    public MMFeedbacks CorrectFeedback;

    private string[] NormalQuestionBank = { "If it's March now, how many months will it be until it's November?", "How many continents are there on Earth?", "How many letters are in the word 'xylophone'?", "How many vowels are in the alphabet (y doesn't count)?", "How many arms and legs does an average person have in total?" };
    private int[] NormalAnswerBank = { 8, 7, 9, 5, 4 };
    private string[] HardQuestionBank = { "How many times does the letter 's' appear in the phrase 'Sassafrass Sal's Suspicious Sandwiches'?", "How many points are there on the maple leaf of the Canadian flag?", "What number does this set of Roman numerals represent: 'XIV?", "How many syllables are in the word 'antidisestablishmentarianism'?" };
    private int[] HardAnswerBank = { 12, 11, 14, 12 };
    private string[] SpellingQuestionBank = { "What animal meows?", "What animal moos?", "What animal barks?", "What animal hoots?" };
    private string[] SpellingAnswerBank = { "CAT", "COW", "DOG", "OWL" };
    // Start is called before the first frame update
    void Start()
    {
        int numAnswers = AnswerButtons.Length;
        int correctLocation = Random.Range(0, numAnswers);
        string[] answers = new string[numAnswers];
        int firstNum;
        int secondNum;
        int questionNum;
        int answer;
        int operatorType;
        string question = "";
        List<int> mutations;
        int wrongAnswer;
        switch (QuestionDifficulty) {
            case 0:
                firstNum = Random.Range(2, 10);
                secondNum = Random.Range(2, 10);
                answer = 0;
                operatorType = Random.Range(0, 2);
                question = "";
                switch (operatorType) {
                    case 0:
                        question = firstNum.ToString() + " + " + secondNum.ToString() + " = ?";
                        answer = firstNum + secondNum;
                        break;
                    case 1:
                        if (secondNum > firstNum)
                        {
                            question = secondNum.ToString() + " - " + firstNum.ToString() + " = ?";
                            answer = secondNum - firstNum;
                            break;
                        } else
                        {
                            question = firstNum.ToString() + " - " + secondNum.ToString() + " = ?";
                            answer = firstNum - secondNum;
                            break;
                        }
                }
                mutations = new List<int>{ 2, 3, 4, -2, -3, -4 };
                for (int x = 0; x < numAnswers; x++)
                {
                    if (x == correctLocation)
                    {
                        answers[x] = answer.ToString();
                    } else {
                        int mutationNum = Random.Range(0, mutations.Count);
                        bool valid = false;
                        wrongAnswer = 0;
                        while (!valid) {    //need a non-negative answer
                            wrongAnswer = answer + mutations[mutationNum];
                            if (wrongAnswer < 0)
                            {
                                mutationNum -= 1;
                            } else
                            {
                                valid = true;
                            }
                        }
                        answers[x] = wrongAnswer.ToString();
                        mutations.RemoveAt(mutationNum);
                    }
                }
                break;

            case 1:
                firstNum = Random.Range(2, 10);
                secondNum = Random.Range(2, 10);
                answer = 0;
                operatorType = Random.Range(0, 2);
                question = "";
                switch (operatorType)
                {
                    case 0:
                        question = firstNum.ToString() + " x " + secondNum.ToString() + " = ?";
                        answer = firstNum * secondNum;
                        break;
                    case 1:
                        int multipliedNumbers = firstNum * secondNum;
                        question = multipliedNumbers.ToString() + " € " + firstNum.ToString() + " = ?";
                        answer = secondNum;
                        break;
                }
                mutations = new List<int> { 1, 2, 3, -1, -2, -3};
                for (int x = 0; x < numAnswers; x++)
                {
                    if (x == correctLocation)
                    {
                        answers[x] = answer.ToString();
                    }
                    else
                    {
                        int mutationNum = Random.Range(0, mutations.Count);
                        bool valid = false;
                        wrongAnswer = 0;
                        while (!valid)
                        {    //need a non-negative answer
                            wrongAnswer = answer + mutations[mutationNum];
                            if (wrongAnswer < 0)
                            {
                                mutationNum -= 1;
                            }
                            else
                            {
                                valid = true;
                            }
                        }
                        answers[x] = wrongAnswer.ToString();
                        mutations.RemoveAt(mutationNum);
                    }
                }
                break;

            case 2:
                questionNum = Random.Range(0, NormalQuestionBank.Length);
                question = NormalQuestionBank[questionNum];
                answer = NormalAnswerBank[questionNum];
                mutations = new List<int> { -1, -2, 1, 2 };
                for (int x = 0; x < numAnswers; x++)
                {
                    if (x == correctLocation)
                    {
                        answers[x] = answer.ToString();
                    }
                    else
                    {
                        int mutationNum = Random.Range(0, mutations.Count);
                        wrongAnswer = answer + mutations[mutationNum];
                        answers[x] = wrongAnswer.ToString();
                        mutations.RemoveAt(mutationNum);
                    }
                }
                break;
            case 3:
                questionNum = Random.Range(0, HardQuestionBank.Length);
                question = HardQuestionBank[questionNum];
                answer = HardAnswerBank[questionNum];
                mutations = new List<int> { -1, -2, 1, 2 };
                for (int x = 0; x < numAnswers; x++)
                {
                    if (x == correctLocation)
                    {
                        answers[x] = answer.ToString();
                    }
                    else
                    {
                        int mutationNum = Random.Range(0, mutations.Count);
                        wrongAnswer = answer + mutations[mutationNum];
                        answers[x] = wrongAnswer.ToString();
                        mutations.RemoveAt(mutationNum);
                    }
                }
                break;
        }

        QuestionPrompt.text = question;
        for (int x = 0; x < numAnswers; x++)
        {
            AnswerPrompts[x].text = answers[x];
            if (x == correctLocation)
            {
                AnswerButtons[x].OnActivation = CorrectAction;
                MMFeedbacks targetFeedbacks = AnswerButtons[x].gameObject.GetComponent<MMFeedbacks>();
                targetFeedbacks.Feedbacks = CorrectFeedback.Feedbacks;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*GameObject highlightedObject = EventSystem.current.currentSelectedGameObject;
        timeDisplay.enabled = false;
        boilerPlate.enabled = false;
        if (highlightedObject == null)
        {
            return;
        }

        GameObject parentObject = highlightedObject.transform.parent.gameObject.transform.parent.gameObject;
        if (parentObject == null)
        {
            return;
        }

        LevelNumberSelector selector = parentObject.GetComponent<LevelNumberSelector>();
        if (selector == null)
        {
            return;
        }

        timeDisplay.enabled = true;
        boilerPlate.enabled = true;

        float bestTime = saver.GetBestTime(selector.LevelNumber);
        if (bestTime == -1f)
        {
            timeDisplay.text = "N/A";
            return;
        }

        timeDisplay.text = bestTime.ToString("F2");
        Debug.Log("succeeded! " + selector.LevelNumber);*/
        }
    }
