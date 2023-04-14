using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{

	public class CharacterTrackHealth : CharacterAbility
    {
		protected GameObject healthBar;
        protected Health characterHealth;

        protected override void Initialization()
        {
            base.Initialization();
            healthBar = GameObject.Find("UICamera/Canvas/HUD/HealthBar");
            characterHealth = this.GetComponent<Health>();
		}

        public virtual void Update()
        {
            if (healthBar != null && characterHealth != null) {
                if (characterHealth.CurrentHealth < 20)
                {
                    healthBar.transform.GetChild(1).gameObject.SetActive(false);
                } else
                {
                    healthBar.transform.GetChild(1).gameObject.SetActive(true);
                }

                if (characterHealth.CurrentHealth < 10)
                {
                    healthBar.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    healthBar.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

	}
}
