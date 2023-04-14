using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	public struct ElectrocuteEvent
	{
		public float stunTime;

		/// <summary>
		/// Initializes a ElectrocuteEvent
		/// </summary>
		/// <param name="enemyCount"></param>
		public ElectrocuteEvent(float time)
		{
			stunTime = time;
		}

		static ElectrocuteEvent e;
		public static void Trigger(float time)
		{
			e.stunTime = time;
			MMEventManager.TriggerEvent(e);
		}
	}

	[MMHiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Player Debuffs")] 
	public class PlayerDebuffs : CharacterAbility, MMEventListener<ElectrocuteEvent>
	{
		protected float invincibilityLength = 3f;

		protected float electrocutedTime = 0f;
		protected float invincibleTime = 0f;
		protected CharacterHorizontalMovement movement;

        protected override void Initialization()
		{
			base.Initialization();
			electrocutedTime = 0f;
			movement = this.GetComponent<CharacterHorizontalMovement>();
		}

		public override void ProcessAbility()
		{
			base.ProcessAbility();
			if (electrocutedTime > 0f)
            {
				_controller.SetHorizontalForce(0f);
				_controller.SetVerticalForce(0f);
				electrocutedTime -= Time.deltaTime;
				if (electrocutedTime <= 0f)
                {
					_controller.GravityActive(true);
					_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
					invincibleTime = invincibilityLength;
				}
            }

			if (invincibleTime > 0f)
            {
				invincibleTime -= Time.deltaTime;
            }
		}

		public virtual void OnMMEvent(ElectrocuteEvent e)
		{
			if (invincibleTime <= 0f && electrocutedTime <= 0f)
			{
				_condition.ChangeState(CharacterStates.CharacterConditions.Stunned);
				_controller.SetHorizontalForce(0f);
				_controller.SetVerticalForce(0f);
				_controller.GravityActive(false);
				Debug.Log("condition: " + _condition.CurrentState);
				electrocutedTime = e.stunTime;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.MMEventStartListening<ElectrocuteEvent>();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.MMEventStopListening<ElectrocuteEvent>();
		}
	}
}
