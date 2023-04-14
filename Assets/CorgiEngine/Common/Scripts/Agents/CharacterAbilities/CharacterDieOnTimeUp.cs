using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{

	public class CharacterDieOnTimeUp : MonoBehaviour, MMEventListener<TimeUpEvent>
    {
        protected Health CharacterHealth;

        void Start()
        {
            CharacterHealth = this.GetComponent<Health>();
        }

		public virtual void OnMMEvent(TimeUpEvent e)
		{
			CharacterHealth.Kill();
		}

		/// <summary>
		/// OnEnable, we start listening to events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<TimeUpEvent>();
		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<TimeUpEvent>();
		}
	}
}
