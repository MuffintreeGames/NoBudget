using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace NoBudget
{
	public struct MusicPlayerEvent
	{
		public string trackName;
		public MusicPlayerEvent(string track)
		{
			trackName = track;
		}

		static MusicPlayerEvent e;
		public static void Trigger(string track)
		{
			e.trackName = track;
			MMEventManager.TriggerEvent(e);
		}
	}

	public class MusicPlayer : MonoBehaviour, MMEventListener<MusicPlayerEvent>
	{

		[SerializeField]
		private MMFeedbacks CutsceneMusic;

		[SerializeField]
		private MMFeedbacks LevelMusic;

		private string currentTrack = "None";

		void Awake()
		{
			GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

			if (objs.Length > 1)
			{
				Destroy(this.gameObject);
			}

			DontDestroyOnLoad(this.gameObject);
			currentTrack = "None";
		}

		public virtual void OnMMEvent(MusicPlayerEvent e)
		{
			if (e.trackName != currentTrack)
			{
				MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.StopTrack, MMSoundManager.MMSoundManagerTracks.Music);
				switch (currentTrack)
                {
					case "Level1":
						LevelMusic?.StopFeedbacks();
						break;
					case "Cutscene1":
						Debug.Log("stopping feedbacks");
						CutsceneMusic?.StopFeedbacks();
						break;
                }
			}

			switch (e.trackName)
			{
				case "Level1":
					if (currentTrack != "Level1")
					{
						Debug.Log("playing level 1 music");
						currentTrack = "Level1";
						LevelMusic?.PlayFeedbacks();
					}
					break;
				case "Cutscene1":
					if (currentTrack != "Cutscene1")
					{
						Debug.Log("playing cutscene music");
						currentTrack = "Cutscene1";
						CutsceneMusic?.PlayFeedbacks();
					}
					break;
				default:
					Debug.Log("stopping music");
					currentTrack = "None";
					break;
			}
		}

		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MusicPlayerEvent>();
		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MusicPlayerEvent>();
		}
	}
}
