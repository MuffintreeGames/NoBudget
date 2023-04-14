using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{

	public class CharacterTrackSpeed : CharacterAbility
    {
		protected MMProgressBar speedBar;

        protected override void Initialization()
        {
            base.Initialization();
            speedBar = GameObject.Find("UICamera/Canvas/SpeedBar").GetComponent<MMProgressBar>();
		}

        public override void ProcessAbility()
        {
            base.ProcessAbility();
            float currentSpeedPercentage = Mathf.Abs(_controller.Speed.x) / _controller.Parameters.MaxVelocity.x;
            currentSpeedPercentage = Mathf.Min(100f, currentSpeedPercentage);
            speedBar.SetBar01(currentSpeedPercentage);
        }

	}
}
