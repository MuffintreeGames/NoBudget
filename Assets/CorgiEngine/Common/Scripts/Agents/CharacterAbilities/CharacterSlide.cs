using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// Add this component to a character and it'll be able to crouch and crawl
	/// Animator parameters : Crouching, Crawling
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Slide")]
	public class CharacterSlide : CharacterAbility
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows a character to slide. Sliding requires a certain amount of speed to start, and lowers speed while in the sliding state. The collider will resize when sliding."; }

		/// the deceleration of the character when it's sliding; how quickly you lose speed
		[Tooltip("the deceleration of the character when it's sliding; how quickly you lose speed")]
		public float SlideDeceleration = 1f;
		/// the minimum speed necessary to start a slide
		[Tooltip("the minimum speed necessary to start a slide")]
		public float MinimumSlideStartSpeed = 6f;
		/// the minimum speed necessary to maintain a slide. If in a tunnel, will remain at this speed until leaving the tunnel
		[Tooltip("the minimum speed necessary to maintain a slide. If in a tunnel, will remain at this speed until leaving the tunnel")]
		public float MinimumSlideHoldSpeed = 2f;

		/// the size to apply to the collider when sliding
		/// note that changing the width of your collider when sliding will likely result in glitches when initiating a slide on a slope, it's best not to change it
		[Tooltip("the size to apply to the collider when crouched (if ResizeColliderWhenCrouched is true, otherwise this will be ignored)" +
			"note that changing the width of your collider when crouching will likely result in glitches when initiating a crouch on a slope, it's best not to change it")]
		public Vector2 SlidingBoxColliderSize = new Vector2(1, 1);

		/// if this is true, the character is sliding and has an obstacle over its head that prevents it from getting back up again. Minimum speed should be maintained until they can escape
		[MMReadOnly]
		[Tooltip("if this is true, the character is sliding and has an obstacle over its head that prevents it from getting back up again")]
		public bool InATunnel;

		//public MMFeedbacks SlideHoldFeedback;

		[Header("Cinemachine")]

		// animation parameters
		protected const string _slidingAnimationParameterName = "Sliding";
		protected int _slidingAnimationParameter;
		protected bool _wasInATunnelLastFrame;
		protected bool _sliding = false;
		protected Health characterHealth;

		// slide icon on GUI
		protected Image slideIcon;

		protected GameObject slideDamage;
		protected bool slidePermitted;
		protected float originalDeceleration;

		/// <summary>
		/// On Start(), we set our tunnel flag to false
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			InATunnel = false;
			slideDamage = this.transform.Find("SlideDamageOnTouch").gameObject;
			slideIcon = GameObject.Find("UICamera/Canvas/AbilityIcons/Slide").GetComponent<Image>();
		}

		/// <summary>
		/// Every frame, we check if we're crouched and if we still should be
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			CheckSlidePermitted();
			slideIcon.enabled = slidePermitted;
			if (_sliding)
            {
				PerformSlideActions();
            }
			CheckExitSlide();
		}

		//check if player is going fast enough to slide, and is in a slide-ready state
		protected void CheckSlidePermitted()
        {
			if (!AbilityAuthorized // if the ability is not permitted
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) // or if we're not in our normal stance
				|| (!_controller.State.IsGrounded) // or if we're not grounded
				|| (_movement.CurrentState == CharacterStates.MovementStates.Gripping)) // or if we're gripping
			{
				// we do not allow sliding
				slidePermitted = false;
				return;
			}

			if (Mathf.Abs(_controller.Speed.x) < MinimumSlideStartSpeed)    //have to be at the minimum speed to do the slide
			{
				slidePermitted = false;
				return;
			}
			slidePermitted = true;
		}

		/// <summary>
		/// At the start of the ability's cycle, we check if we're pressing the slide button. If yes, we call Slide()
		/// </summary>
		protected override void HandleInput()
		{
			// Slide Detection : if the player is pressing "Slide", if the character is grounded, and if the character is moving fast enough, perform a slide
			if (_inputManager.SlideButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				Slide();
			}
		}

		/// <summary>
		/// If we're currently sliding, we decelerate the character (and kill enemies? Might be better done in enemy code)
		/// </summary>
		protected virtual void PerformSlideActions()
        {
			if (_characterHorizontalMovement != null)
			{
				//_characterHorizontalMovement.MovementSpeed = CrawlSpeed;
			}
		}

		public virtual void InitiateSlide()
        {
			Slide();
        }

		/// <summary>
		/// If we're pressing the slide button, we check if we can slide, and change states accordingly
		/// </summary>
		protected virtual void Slide()
		{
			if (!slidePermitted)
            {
				return;
            }

			// if this is the first time we're here, we trigger our sounds
			if ((_movement.CurrentState != CharacterStates.MovementStates.Sliding))
			{
				// we play the slide start sound 
				PlayAbilityStartFeedbacks();
				MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Slide, MMCharacterEvent.Moments.Start);
			}

			// we set the character's state to Sliding
			_movement.ChangeState(CharacterStates.MovementStates.Sliding);
			_sliding = true;

			// we resize our collider to match the new shape of our character (it's usually smaller when sliding)

			_controller.ResizeCollider(SlidingBoxColliderSize);
			Invoke("RecalculateRays", Time.deltaTime * 10);
			originalDeceleration = _controller.Parameters.SpeedDecelerationOnGround;

			_controller.Parameters.SpeedDecelerationOnGround = SlideDeceleration;

			// we change our character's deceleration and prevent movement
			if (_characterHorizontalMovement != null)
			{
				_characterHorizontalMovement.ReadInput = false;
				_characterHorizontalMovement.SetHorizontalMove(0);
			}


			if (slideDamage != null)	//allow character to inflict damage while sliding
            {
				slideDamage.SetActive(true);
			}

		}

		/// <summary>
		/// Every frame, we check to see if we should exit the Sliding state
		/// </summary>
		protected virtual void CheckExitSlide()
		{
			if (_inputManager == null)
			{
				if ((_movement.CurrentState == CharacterStates.MovementStates.Sliding))
				{
					ExitSlide();
				}
			}

			if ((_movement.CurrentState == CharacterStates.MovementStates.Sliding))
			{
				// but we're not pressing slide anymore, or we're not grounded anymore, or we're not going fast enough
				if ((!_controller.State.IsGrounded) || (_inputManager.SlideButton.State.CurrentState != MMInput.ButtonStates.ButtonPressed && _inputManager.SlideButton.State.CurrentState != MMInput.ButtonStates.ButtonDown) || (Mathf.Abs(_controller.Speed.x) < MinimumSlideHoldSpeed))
				{
					ExitSlide();
				}
			} else
            {
				if (!_characterHorizontalMovement.ReadInput)
                {
					ExitSlide();
                }
			}
		}

		/// <summary>
		/// Exits the crouched state
		/// </summary>
		protected virtual void ExitSlide()
		{
			// we cast a raycast above to see if we have room enough to go back to normal size
			InATunnel = !_controller.CanGoBackToOriginalSize();
			_wasInATunnelLastFrame = InATunnel;

			// if the character is not in a tunnel, we can go back to normal size
			if (!InATunnel)
			{
				// we return to normal walking speed
				/*if (_characterHorizontalMovement != null)
				{
					_characterHorizontalMovement.ResetHorizontalSpeed();
				}*/

				// we play our exit feedback
				StopStartFeedbacks();
				PlayAbilityStopFeedbacks();
				MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Slide, MMCharacterEvent.Moments.End);

				// we go back to Idle state and reset our collider's size

				_movement.ChangeState(CharacterStates.MovementStates.Idle);

				_sliding = false;
				_controller.Parameters.SpeedDecelerationOnGround = originalDeceleration;

				_controller.ResetColliderSize();
				Invoke("RecalculateRays", Time.deltaTime * 10);
				Debug.Log("End slide");
				_controller.ResetParameters();
				// reset character movement
				if (_characterHorizontalMovement != null)
				{
					_characterHorizontalMovement.ReadInput = true;
				}

				if (slideDamage != null)    //deactivate damaging hitbox
				{
					slideDamage.SetActive(false);
				}
			} else
            {
				if (Mathf.Abs(_controller.Speed.x) < MinimumSlideHoldSpeed)
                {
					_controller.Parameters.SpeedDecelerationOnGround = 0;
				}

			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_slidingAnimationParameterName, AnimatorControllerParameterType.Bool, out _slidingAnimationParameter);
		}

		/// <summary>
		/// At the end of the ability's cycle, we send our current crouching and crawling states to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _slidingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Sliding), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
		}

		/// <summary>
		/// Recalculates the raycast's origin points.
		/// </summary>
		protected virtual void RecalculateRays()
		{
			_character.RecalculateRays();
		}

		/// <summary>
		/// On reset ability, we cancel all the changes made
		/// </summary>
		public override void ResetAbility()
		{
			base.ResetAbility();
			if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
			{
				ExitSlide();
			}

			if (_animator != null)
			{
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _slidingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			}
		}
	}
}
