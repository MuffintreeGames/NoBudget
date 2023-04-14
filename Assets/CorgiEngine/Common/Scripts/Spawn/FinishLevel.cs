using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    public struct PostLevelSaveEvent
    {
        public int Level;
        public float Time;
        public PostLevelSaveEvent(int level, float time)
        {
            Level = level;
            Time = time;
        }

        static PostLevelSaveEvent e;
        public static void Trigger(int level, float time)
        {
            e.Level = level;
            e.Time = time;
            MMEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// Add this class to a trigger and it will send your player to the next level
    /// </summary>
    [AddComponentMenu("Corgi Engine/Spawn/Finish Level")]
	public class FinishLevel : ButtonActivated, MMEventListener<ExitUnlockedEvent>
    {
        [MMInspectorGroup("Finish Level", true, 22)]

        /// the (exact) name of the level to go to 
		[Tooltip("the (exact) name of the level to go to ")]
		public string LevelName;
        /// the delay (in seconds) before actually redirecting to a new scene
		[Tooltip("the delay (in seconds) before actually redirecting to a new scene")]
		public float DelayBeforeTransition = 0f;
        /// the internal numbering of this level, used for save data
        [Tooltip("the internal numbering of this level, used for save data")]
        public int levelNumber;

        [MMInspectorGroup("MMFader Transition", true, 25)]

        /// if this is true, a fade to black will occur when teleporting
		[Tooltip("if this is true, a fade to black will occur when teleporting")]
        public bool TriggerFade = false;
        /// the ID of the fader to target
        [MMCondition("TriggerFade", true)]
        [Tooltip("the ID of the fader to target")]
        public int FaderID = 0;
        /// the curve to use to fade to black
        [MMCondition("TriggerFade", true)]
        [Tooltip("the curve to use to fade to black")]
        public MMTweenType FadeTween = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

        [MMInspectorGroup("Freeze", true, 27)]

        /// whether or not time should be frozen during the transition
        [Tooltip("whether or not time should be frozen during the transition")]
        public bool FreezeTime = false;
        /// whether or not the character should be frozen (input blocked) for the duration of the transition
        [Tooltip("whether or not the character should be frozen (input blocked) for the duration of the transition")]
        public bool FreezeCharacter = true;

        protected WaitForSeconds _delayWaitForSeconds;
        protected Character _character;

        [MMInspectorGroup("Lock", true, 28)]

        /// <summary>
        ///  whether or not the exit should be unlocked by default
        /// </summary>
        [Tooltip("whether or not the exit should be unlocked by default")]
        public bool ExitUnlocked = false;
        /// <summary>
        /// feedbacks to play when exit is unlocked
        /// </summary>
        [Tooltip("feedbacks to play when exit is unlocked")]
        public MMFeedbacks UnlockFeedbacks;

        /// <summary>
        /// On initialization, we init our delay
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
            _delayWaitForSeconds = new WaitForSeconds(DelayBeforeTransition);
        }

        /// <summary>
        /// When the button is pressed we start the dialogue
        /// </summary>
        public override void TriggerButtonAction(GameObject instigator)
        {
            if (!ExitUnlocked)  //exit must first be unlocked by killing all enemies
            {
                return;
            }
            if (instigator.GetComponent<Character>() != null)
            {
                _character = instigator.GetComponent<Character>();
            }
			if (!CheckNumberOfUses())
			{
				return;
			}

			base.TriggerButtonAction (instigator);

            float completionTime = -1f;
            LevelTimer levelTimer = LevelManager.Instance.gameObject.GetComponent<LevelTimer>();
            if (levelTimer != null)
            {
                completionTime = levelTimer.timeElapsed;
                completionTime = Mathf.Round(completionTime * 100.0f) * 0.01f;
            }
            PostLevelSaveEvent.Trigger(levelNumber, completionTime);
            StartCoroutine(GoToNextLevelCoroutine());
			ActivateZone ();
		}	
        
        /// <summary>
        /// A coroutine used to handle the finish level sequence
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator GoToNextLevelCoroutine()
        {
            // we trigger a fade if needed
            if (TriggerFade && (DelayBeforeTransition > 0f))
            {
                MMFadeInEvent.Trigger(DelayBeforeTransition, FadeTween, FaderID, false, LevelManager.Instance.Players[0].transform.position);
            }

            // we freeze time if needed
            if (FreezeTime)
            {
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0f, 0f, false, 0f, true);
            }

            // we freeze our character if needed
            if (FreezeCharacter && (_character != null))
            {
                _character.Freeze();
            }

            // we wait for the duration of the specified delay
            yield return _delayWaitForSeconds;

            // finally we move to the next level
            GoToNextLevel();
        }

		/// <summary>
		/// Loads the next level
		/// </summary>
	    public virtual void GoToNextLevel()
	    {
	    	if (LevelManager.HasInstance)
	    	{
				LevelManager.Instance.GotoLevel(LevelName, (DelayBeforeTransition == 0f));
	    	}
	    	else
	    	{
		        MMSceneLoadingManager.LoadScene(LevelName);
			}
	    }

        public virtual void OnMMEvent(ExitUnlockedEvent e)
        {
            ExitUnlocked = e.ExitUnlocked;
            UnlockFeedbacks?.PlayFeedbacks();
        }

        /// <summary>
        /// OnEnable, we start listening to events.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<ExitUnlockedEvent>();
        }

        /// <summary>
        /// OnDisable, we stop listening to events.
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<ExitUnlockedEvent>();
        }
    }
}