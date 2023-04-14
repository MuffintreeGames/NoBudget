﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.EventSystems;

namespace MoreMountains.Tools
{
    /// <summary>
    /// Add this bar to an object and link it to a bar (possibly the same object the script is on), and you'll be able to resize the bar object based on a current value, located between a min and max value.
    /// See the HealthBar.cs script for a use case
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/GUI/MMGetFocusOnEnable")]
    public class MMGetFocusOnEmpty : MonoBehaviour
	{
        protected virtual void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject, null);
            }
        }
	}
}