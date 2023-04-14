using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to a character and it'll have to do a QTE every several seconds to not die
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character QTE")] 
	public class CharacterQTE : CharacterAbility 
	{

		protected enum QTEButton { Jump, Dash, Slide, Shoot};

		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "Causes this character to automatically pause the game (but not the timer) every several seconds until a random button is pushed. The character will die if the button is not pushed quickly."; }
		protected bool qteGoing = false;
		protected float timeBetweenQtes = 6f;
		protected float timeUntilQte;
		protected float timeLeftInQte;
		protected Text qtePrompt;
		protected Text qteTimer;
		protected QTEButton targetButton;
		protected LevelManager LManager;

		protected override void Initialization()
		{
			base.Initialization();
			qtePrompt = GameObject.Find("UICamera/Canvas/QTEPrompt").GetComponent<Text>();
			qteTimer = GameObject.Find("UICamera/Canvas/QTETimer").GetComponent<Text>();
			LManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
			timeUntilQte = timeBetweenQtes;
		}

		void Update()
		{
			if (qteGoing)
			{
				timeLeftInQte -= Time.unscaledDeltaTime;
				qteTimer.text = timeLeftInQte.ToString("F2");
				if (timeLeftInQte <= 0f)
                {
					EndQTE();
					LManager.KillPlayer(LManager.Players[0]);
                }
			} else
            {
				timeUntilQte -= Time.deltaTime;
				if (timeUntilQte <= 0f)
                {
					TriggerQTE();
					timeUntilQte = timeBetweenQtes;
                }
            }
		}

		/// <summary>
		/// Every frame, we check the input to see if we need to pause/unpause the game
		/// </summary>
		/// 
		protected override void HandleInput()
		{
            if (_character.CharacterType != Character.CharacterTypes.Player)
            {
                return;
            }
			if (_inputManager.FeatureTestButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)				
			{
				if (!qteGoing) {
					TriggerQTE();
				} else
                {
					EndQTE();
                }
			}
			if (qteGoing)
            {
				switch (targetButton)
                {
					case QTEButton.Jump:
						if (_inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
						{
							EndQTE();
						}
						break;
					case QTEButton.Dash:
						if (_inputManager.DashButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
						{
							EndQTE();
						}
						break;
					case QTEButton.Slide:
						if (_inputManager.SlideButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
						{
							EndQTE();
						}
						break;
					case QTEButton.Shoot:
						if (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
						{
							EndQTE();
						}
						break;
				}
            }
		}

		/// <summary>
		/// When a QTE begins, send out a message to pause the game but not the timer
		/// </summary>
		protected virtual void TriggerQTE()
		{
			if (!AbilityAuthorized
			&& (_condition.CurrentState == CharacterStates.CharacterConditions.Normal || _condition.CurrentState == CharacterStates.CharacterConditions.Paused))
			{
				return;
			}
			if (!qteGoing)
			{
				PlayAbilityStartFeedbacks();
				int randomNumber = Random.Range(1, 5);
				switch (randomNumber) {
					case 1:
						targetButton = QTEButton.Jump;
						qtePrompt.text = "PRESS THE JUMP BUTTON TO NOT DIE!";
						break;
					case 2:
						targetButton = QTEButton.Dash;
						qtePrompt.text = "PRESS THE DASH BUTTON TO NOT DIE!";
						break;
					case 3:
						targetButton = QTEButton.Slide;
						qtePrompt.text = "PRESS THE SLIDE BUTTON TO NOT DIE!";
						break;
					case 4:
						targetButton = QTEButton.Shoot;
						qtePrompt.text = "PRESS THE SHOOT BUTTON TO NOT DIE!";
						break;
				}
				timeLeftInQte = 3.00f;
				qteTimer.text = timeLeftInQte.ToString("F2");
				qtePrompt.enabled = true;
				qteTimer.enabled = true;
				qteGoing = true;
				// we trigger a Pause event for the GameManager and other classes that could be listening to it too
				PlayAbilityStartFeedbacks();
				CorgiEngineEvent.Trigger(CorgiEngineEventTypes.BeginQTE);
				PauseCharacter();
			}
		}

		protected virtual void EndQTE()
        {
			StopStartFeedbacks();
			if (timeLeftInQte > 0f)
            {
				PlayAbilityStopFeedbacks();
			}
			qtePrompt.enabled = false;
			qteTimer.enabled = false;
			qteGoing = false;
			CorgiEngineEvent.Trigger(CorgiEngineEventTypes.EndQTE);
			UnPauseCharacter();
		}

		/// <summary>
		/// Puts the character in the pause state
		/// </summary>
		public virtual void PauseCharacter()
		{
            if (!this.enabled)
            {
                return;
            }
			_condition.ChangeState(CharacterStates.CharacterConditions.Paused);
            _controller.enabled = false;
		}

		/// <summary>
		/// Restores the character to the state it was in before the pause.
		/// </summary>
		public virtual void UnPauseCharacter()
        {
            if (!this.enabled)
            {
                return;
            }
            _condition.RestorePreviousState();
            _controller.enabled = true;
        }

		protected virtual void OnDisable()
        {
			base.OnDisable();
			CorgiEngineEvent.Trigger(CorgiEngineEventTypes.EndQTE);
		}
	}
}
