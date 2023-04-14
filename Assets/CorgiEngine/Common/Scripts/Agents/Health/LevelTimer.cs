using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{
    public class LevelTimer : MonoBehaviour, MMEventListener<CorgiEngineEvent>
    {
        /// time limit for the level (in seconds)
        public float TimeLimit = 30f;
        public float timeElapsed;

        protected bool running = false;
        protected MMProgressBar TimeBar;
        protected LevelManager LManager;
        protected bool unscaled = false;

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
                    if (unscaled)
                    {
                        timeElapsed += Time.unscaledDeltaTime;
                    }
                    else
                    {
                        timeElapsed += Time.deltaTime;
                    }
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

        protected virtual void SwitchToUnscaledTime()
        {
            unscaled = true;
        }

        protected virtual void SwitchToRegularTime()
        {
            unscaled = false;
        }

        public virtual void OnMMEvent(CorgiEngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                
                case CorgiEngineEventTypes.BeginQTE:
                    SwitchToUnscaledTime();
                    break;
                case CorgiEngineEventTypes.EndQTE:
                    SwitchToRegularTime();
                    break;
            }
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<CorgiEngineEvent>();
        }

        /// <summary>
        /// OnDisable, we stop listening to events.
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<CorgiEngineEvent>();
        }
    }
}
