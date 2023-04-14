using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using UnityEngine.Video;
using UnityEngine.UI;

namespace MoreMountains.CorgiEngine
{
	public class TutorialPopup : MonoBehaviour, MMEventListener<TutorialPopupEvent>
	{
		private GameObject container;
		private Text tutorialTitleText;
		private Text tutorialBodyText;
		private VideoPlayer tutorialVideoPlayer;
		
		public void Awake()
        {
			container = this.transform.Find("Container").gameObject;
			tutorialTitleText = container.transform.Find("TutorialTitle").gameObject.GetComponent<Text>();
			tutorialBodyText = container.transform.Find("TutorialMessage").gameObject.GetComponent<Text>();
			tutorialVideoPlayer = container.transform.Find("VideoFeed").gameObject.GetComponent<VideoPlayer>();
		}


		public virtual void CloseWindow()
		{
			container.SetActive(false);
			CloseTutorialEvent.Trigger(true);
		}

		public virtual void OnMMEvent(TutorialPopupEvent e)
		{
			tutorialTitleText.text = e.TutorialName;
			tutorialBodyText.text = e.TutorialText;
			tutorialVideoPlayer.clip = e.TutorialVideo;
			container.SetActive(true);
		}

		/// <summary>
		/// OnEnable, we start listening to events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<TutorialPopupEvent>();
		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<TutorialPopupEvent>();
		}
	}
}