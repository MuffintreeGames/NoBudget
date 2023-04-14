using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{
    public class LevelTimer : MonoBehaviour
    {
        /// time limit for the level (in seconds)
        public float TimeLimit = 30f;

        protected bool running = false;
        protected MMProgressBar TimeBar;
        protected float timeElapsed;
        protected LevelManager LManager;

        void Start()
        {
            TimeBar = GameObject.Find("/UICamera/Canvas/HUD/TimeBar").GetComponent<MMProgressBar>();
            timeElapsed = 0f;
            LManager = this.GetComponent<LevelManager>();
        }

        void Update()
        {
            if (TimeBar != null)
            {
                if (running)
                {
                    timeElapsed += Time.deltaTime;
                    Debug.Log(timeElapsed);
                    float remainingPercent = (TimeLimit - timeElapsed) / TimeLimit;
                    if (remainingPercent <= 0) {
                        LManager.KillPlayer(LManager.Players[0]);
                        StopLevelTimer();
                    } else {
                        TimeBar.SetBar01(remainingPercent);
                    }
                }
            }
        }


        public virtual void StartLevelTimer()
        {
            running = true;
        }

        public virtual void StopLevelTimer()
        {
            running = false;
        }
    }
}
