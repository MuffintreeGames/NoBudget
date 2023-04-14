using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{

    /// <summary>
    /// This component allows your character to use a grappling hook weapon to move around. It will not work properly without one equipped
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Grapple")]
    public class CharacterGrapple : CharacterAbility, MMEventListener<StartGrappleEvent>
    {
        public override string HelpBoxText() { return "This component allows your character to use a grappling hook weapon to move around. It will not work properly without one equipped."; }

        /// the speed at which the character should move while grappling
        [Tooltip("the speed at which the character should move while grappling")]
        public float GrappleSpeed = 12f;

        ///  the layers which will interrupt a grapple in progress
        [Tooltip("the layers which will interrupt a grapple in progress")]
        public LayerMask GrappleEndLayers;

        //protected float _horizontalMovement;
        //protected float _verticalMovement;
        protected bool _grappling;
        
        // animation parameters
        protected const string _grapplingAnimationParameterName = "Grappling";
        protected const string _grapplingSpeedAnimationParameterName = "GrapplingSpeed";
        protected int _grapplingAnimationParameter;
        protected int _grapplingSpeedAnimationParameter;

        protected float SpeedX = 0f;
        protected float SpeedY = 0f;

        protected GrappleProjectile hook;

        /// <summary>
        /// On Start, we initialize our flight if needed
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _grappling = false;
        }

        /// <summary>
        /// Looks for hztal and vertical input, and for flight button if needed
        /// </summary>
        protected override void HandleInput()
        {
           if (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                StopGrapple();
            }
        }
        /// <summary>
        /// Starts the flight sequence
        /// </summary>
        public virtual void StartGrapple(GrappleProjectile targetProjectile)
        {
            if ((!AbilityAuthorized) // if the ability is not permitted
                || (_movement.CurrentState == CharacterStates.MovementStates.Dashing) // or if we're dashing
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if we're in the gripping state
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not in normal conditions
            {
                return;
            }

            // if this is the first time we're here, we trigger our sounds
            if (_movement.CurrentState != CharacterStates.MovementStates.Grappling)
            {
                // we play the jetpack start sound 
                PlayAbilityStartFeedbacks();
                MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Grapple, MMCharacterEvent.Moments.Start);
                _grappling = true;
                hook = targetProjectile;
            }

            _controller.DisableSpeedLimits();
            // we set the various states
            _movement.ChangeState(CharacterStates.MovementStates.Grappling);

            _controller.GravityActive(false);
        }

        /// <summary>
        /// Stops the flight
        /// </summary>
        public virtual void StopGrapple()
        {
            if (_grappling)
            {
                StopStartFeedbacks();
                PlayAbilityStopFeedbacks();
                MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Grapple, MMCharacterEvent.Moments.End);
                hook.Invoke("Destroy", 0);
                _controller.GravityActive(true);
                _grappling = false;
                _controller.EnableSpeedLimits();
                _movement.RestorePreviousState();
            }
        }

        /// <summary>
        /// On Update, checks if we should stop flying
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            HandleMovement();
        }

        public virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (_grappling)
            {
                if (collider.gameObject == hook.gameObject)
                {
                    StopGrapple();
                }

                if (!MMLayers.LayerInLayerMask(collider.gameObject.layer, GrappleEndLayers))
                {
                    return;
                }
                if (collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatforms") || collider.gameObject.layer == LayerMask.NameToLayer("MovingOneWayPlatforms"))   //one-way platforms should only stop us if we're moving down
                {
                    if (SpeedY >= 0)
                    {
                        return;
                    }
                }
                if (SpeedY < 0 && _controller.State.IsCollidingBelow)
                {
                    StopGrapple();
                    return;
                }
                if (SpeedY > 0 && _controller.State.IsCollidingAbove)
                {
                    StopGrapple();
                    return;
                }
                if (SpeedX > 0 && _controller.State.IsCollidingRight)
                {
                    StopGrapple();
                    return;
                }
                if (SpeedX < 0 && _controller.State.IsCollidingLeft)
                {
                    StopGrapple();
                    return;
                }
                //StopGrapple();
            }
        }


        /// <summary>
        /// Makes the character move in the air
        /// </summary>
        protected virtual void HandleMovement()
        {
            // if we're not walking anymore, we stop our walking sound
            if (_movement.CurrentState != CharacterStates.MovementStates.Grappling && _startFeedbackIsPlaying)
            {
                StopStartFeedbacks();
            }

            // if movement is prevented, or if the character is dead/frozen/can't move, we exit and do nothing
            if (!AbilityAuthorized
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                return;
            }
           
            
            if (!_grappling)
            {
                return;
            }

            Vector2 currentPosition = this.transform.position;
            Vector2 grappleLocation = hook.transform.position;
            Vector2 distanceToGoal = new Vector2(grappleLocation.x - currentPosition.x, grappleLocation.y - currentPosition.y);

            float totalDistance = Mathf.Abs(distanceToGoal.x) + Mathf.Abs(distanceToGoal.y);
            SpeedX = distanceToGoal.x / totalDistance;
            SpeedY = distanceToGoal.y / totalDistance;
            // If the value of the horizontal axis is positive, the character must face right.
            if (SpeedX > 0.1f)
            {
                if (!_character.IsFacingRight)
                    _character.Flip();
            }
            // If it's negative, then we're facing left
            else if (SpeedX < -0.1f)
            {
                if (_character.IsFacingRight)
                    _character.Flip();
            }
            Vector2 newSpeed = new Vector2(GrappleSpeed * SpeedX, GrappleSpeed * SpeedY);
            _controller.SetUncappedForce(newSpeed);
        }

        /// <summary>
        /// When the character respawns we reinitialize it
        /// </summary>
        protected virtual void OnRevive()
        {
            Initialization();
        }

        /// <summary>
        /// On death the character stops flying if needed
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();
            StopGrapple();
        }

        public virtual void OnMMEvent(StartGrappleEvent e)
        {
            StartGrapple(e.GrappleHook);
        }

        /// <summary>
        /// When the player respawns, we reinstate this agent.
        /// </summary>
        /// <param name="checkpoint">Checkpoint.</param>
        /// <param name="player">Player.</param>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (gameObject.GetComponentInParent<Health>() != null)
            {
                gameObject.GetComponentInParent<Health>().OnRevive += OnRevive;
            }
            this.MMEventStartListening<StartGrappleEvent>();
        }

        /// <summary>
        /// Stops listening for revive events
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_health != null)
            {
                _health.OnRevive -= OnRevive;
            }
            this.MMEventStopListening<StartGrappleEvent>();
        }

        /// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_grapplingAnimationParameterName, AnimatorControllerParameterType.Bool, out _grapplingAnimationParameter);
            RegisterAnimatorParameter(_grapplingSpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _grapplingSpeedAnimationParameter);
        }

        /// <summary>
        /// At the end of each cycle, we send our character's animator the current flying status
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _grapplingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Grappling), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _grapplingSpeedAnimationParameter, Mathf.Abs(_controller.Speed.magnitude), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }

        /// <summary>
        /// On reset ability, we cancel all the changes made
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            StopGrapple();
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _grapplingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _grapplingSpeedAnimationParameter, 0f, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }
    }
}
