using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	public class CloseTutorialButton : MonoBehaviour
	{
		private GameObject tutorialWindow;
		public void Start()
        {
			tutorialWindow = GameObject.Find("TutorialVideo");
        }
		public virtual void CloseWindow()
		{
			tutorialWindow.SetActive(false);
		}
	}
}