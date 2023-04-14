using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{

    /// Add this component to a character and it'll die if its horizontal speed remains below a certain value for too long
	public class CharacterDieIfSlow: CharacterAbility
    {
        protected float maxTimerLength = 2f;
        protected float speedPercentage = 0.6f;
        protected float currentTimerLength;
        protected bool timerActive = false;
        protected float graceTimerLength = 3f;
        protected float graceTimer;
        protected LevelManager LManager;
        protected Text speedPrompt;
        protected Text speedTimer;

        protected override void Initialization()
        {
            base.Initialization();
            LManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            speedPrompt = GameObject.Find("UICamera/Canvas/SpeedPrompt").GetComponent<Text>();
            speedTimer = GameObject.Find("UICamera/Canvas/SpeedTimer").GetComponent<Text>();
            timerActive = false;
            currentTimerLength = 0f;
            graceTimer = graceTimerLength;
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();
            if (graceTimer > 0f)    //few seconds to build speed at the start of the level
            {
                graceTimer -= Time.deltaTime;
                return;
            }
            float currentSpeedPercentage = Mathf.Abs(_controller.Speed.x) / _controller.Parameters.MaxVelocity.x;
            if (currentSpeedPercentage < speedPercentage)
            {
                if (!timerActive)
                {
                    Debug.Log("below speed threshold, starting death timer");
                    PlayAbilityStartFeedbacks();
                    timerActive = true;
                    currentTimerLength = maxTimerLength;
                    speedPrompt.enabled = true;
                    speedTimer.enabled = true;
                } else
                {
                    
                    currentTimerLength -= Time.deltaTime;
                    speedTimer.text = currentTimerLength.ToString("F2");
                    if (currentTimerLength <= 0f)
                    {
                        Debug.Log("time up, killing player");
                        timerActive = false;
                        LManager.KillPlayer(LManager.Players[0]);
                    }
                } 
            } else if (timerActive)
            {
                StopStartFeedbacks();
                timerActive = false;
                Debug.Log("timer deactivated");
                speedPrompt.enabled = false;
                speedTimer.enabled = false;
            } 
        }

	}
}
