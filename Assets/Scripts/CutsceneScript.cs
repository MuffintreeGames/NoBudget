using UnityEngine;
using UnityEngine.UI;
using System;
using Ink.Runtime;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;

public class CutsceneScript : MonoBehaviour {
    public static event Action<Story> OnCreateStory;
	
    void Awake () {
		CreateContentView("default");
		StartStory();
		remainingFeedbackDelay = 0f;
	}

	void Update ()
    {
		if (delayRemaining > 0f)
		{
			delayRemaining -= Time.deltaTime;
		}
		if (appearDelayRemaining > 0f)
		{
			appearDelayRemaining -= Time.deltaTime;
		}

		if (autoAdvanceActive)	//prevent anything from happening while waiting for the next line to be printed
		{
			remainingAutoAdvanceDelay -= Time.deltaTime;
			if (remainingAutoAdvanceDelay <= 0f)
			{
				autoAdvanceActive = false;
				AdvanceStory();
			}
		}
		else
		{

			if (appearDelayRemaining <= 0f && !lineOver)
			{
				PrintNextCharacter();
			}
			if (remainingFeedbackDelay > 0f)
			{
				remainingFeedbackDelay -= Time.deltaTime;
			}
			if (Input.GetButton("Pause"))
			{
				SkipScene();
			}
			if (Input.GetButton("AdvanceText") && delayRemaining <= 0f && canSkipText)
			{
				delayRemaining = delay;
				if (!lineOver)
				{
					DisplayFullLine();
				}
				else
				{
					AdvanceStory();
				}
			}
		}
    }

	// Creates a new Story object with the compiled story which we can then play!
	void StartStory () {
		story = new Story (inkJSONAsset.text);
        if(OnCreateStory != null) OnCreateStory(story);
		qualityCategory = (string)story.variablesState["sprite_quality"];
		StartNextLine();
	}

	void AdvanceStory()
	{
		if (story.canContinue)
        {
			StartNextLine();
		} else
        {
			SceneManager.LoadScene(nextScene);
        }
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void StartNextLine () {
		storyText.text = "";
		lineOver = false;
		currentLine = story.Continue ();
		currentLine = currentLine.Trim();
		placeInLine = 0;
		string speakerName = (string)story.variablesState["speaker_name"];
		speakerBox.text = speakerName;
		UpdateSpeakerSprites();
		if (story.variablesState["glitch_mode"] != null)
		{
			glitchMode = (bool)story.variablesState["glitch_mode"];
		}
		if (glitchMode)
        {
			speakerFeedbacks = GlitchFeedbacks;
			canSkipText = false;
        }
	}

	void PrintNextCharacter()
    {
		if (placeInLine < currentLine.Length)
        {
			if (currentLine[placeInLine] == '!' || currentLine[placeInLine] == '?')	//keep reading until reaching a space, then trigger a delay
            {
				hitSpecialFullStop = true;
				storyText.text += currentLine[placeInLine];
				appearDelayRemaining = appearDelay;
				placeInLine++;
			} else if (currentLine[placeInLine] == '.')	//trigger a delay
            {
				hitSpecialFullStop = false;
				appearDelayRemaining = fullStopDelay;
				storyText.text += currentLine[placeInLine];
				placeInLine++;
			} else if (currentLine[placeInLine] == ' ' && hitSpecialFullStop)
            {
				hitSpecialFullStop = false;
				appearDelayRemaining = fullStopDelay - appearDelay;
            } 
			else if ((currentLine[placeInLine] == '-' || glitchMode) && (placeInLine + 1) == currentLine.Length)
            {
				storyText.text += currentLine[placeInLine];
				autoAdvanceActive = true;
				remainingAutoAdvanceDelay = autoAdvanceDelay;
            } else
            {
				hitSpecialFullStop = false;
				appearDelayRemaining = appearDelay;
				storyText.text += currentLine[placeInLine];
				placeInLine++;
			}
			

			if (remainingFeedbackDelay <= 0f)
			{
				speakerFeedbacks?.PlayFeedbacks();
				remainingFeedbackDelay = feedbackDelay;
			}
        } else
        {
			delayRemaining = delay;
			lineOver = true;
		}
    }

	void DisplayFullLine()
    {
		storyText.text += currentLine.Substring(placeInLine);
		lineOver = true;
    }

	void UpdateSpeakerSprites()
    {
		string activeSpeaker = (string)story.variablesState["active_sprite"];
		Color inactiveSpeakerColor = new Color(0.255f, 0.255f, 0.255f, 1);
		switch (activeSpeaker) {
			case "left":
				leftSprite.color = Color.white;
				rightSprite.color = inactiveSpeakerColor;
				speakerFeedbacks = LeftFeedbacks;
				break;
			case "right":
				rightSprite.color = Color.white;
				leftSprite.color = inactiveSpeakerColor;
				speakerFeedbacks = RightFeedbacks;
				break;
			default:
				Debug.Log("error: activeSpeaker is " + activeSpeaker);
				break;
		}
		string leftSpriteName = (string)story.variablesState["left_sprite"];
		leftResolver.SetCategoryAndLabel(qualityCategory, leftSpriteName);
		string rightSpriteName = (string)story.variablesState["right_sprite"];
		rightResolver.SetCategoryAndLabel(qualityCategory, rightSpriteName);
	}

	// Creates a textbox showing the the line of text
	void CreateContentView (string text) {
		storyText = Instantiate (textPrefab) as Text;
		storyText.text = text;
		storyText.transform.SetParent (textCanvas.transform, false);
	}

	void SkipScene()
    {
		if (SkippableScene)
		{
			SceneManager.LoadScene(nextScene);
		}
    }

	[SerializeField]
	private TextAsset inkJSONAsset = null;
	public Story story;

	[SerializeField]
	private Canvas textCanvas = null;
	[SerializeField]
	private Text speakerBox = null;

	// UI Prefabs
	[SerializeField]
	private Text textPrefab = null;

	[SerializeField]
	private SpriteRenderer rightSprite = null;

	[SerializeField]
	private SpriteRenderer leftSprite = null;

	[SerializeField]
	private UnityEngine.U2D.Animation.SpriteResolver rightResolver = null;

	[SerializeField]
	private UnityEngine.U2D.Animation.SpriteResolver leftResolver = null;

	[SerializeField]
	private string nextScene;

	[SerializeField]
	private MMFeedbacks RightFeedbacks;

	[SerializeField]
	private MMFeedbacks LeftFeedbacks;

	[SerializeField]
	private MMFeedbacks GlitchFeedbacks;

	[SerializeField]
	private bool SkippableScene = true;

	private bool lineOver = false;
	private float delay = 0.2f;
	private float appearDelay = 0.015f;
	private float fullStopDelay = 0.25f;
	private float delayRemaining = 0f;
	private float appearDelayRemaining = 0f;
	private Text storyText;
	private string currentLine;
	private int placeInLine;
	private string qualityCategory = "";
	private MMFeedbacks speakerFeedbacks = null;
	private float feedbackDelay = 0.1f;
	private float remainingFeedbackDelay;
	private bool hitSpecialFullStop = false;
	private bool autoAdvanceActive = false;
	private float autoAdvanceDelay = 0.5f;
	private float remainingAutoAdvanceDelay;
	private bool glitchMode = false;
	private bool canSkipText = true;
}
