using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	public class DisableBasedOnLevelsCleared : MonoBehaviour
	{
		/// the level progress that needs to have been completed to activate button
		[Tooltip("the level progress that needs to have been completed to activate button")]
		public int LevelNumber;

		public void Start()
        {
			int latestLevel = GameObject.Find("GameManagers").GetComponent<GameSaver>().GetLatestLevel();
			if (latestLevel < LevelNumber)
            {
				Debug.Log(latestLevel + " is less than " + LevelNumber + ", disabling button");
				MMTouchButton button = this.transform.Find("Container").transform.Find("Background").GetComponent<MMTouchButton>();
				button.DisableButton();
            } else
            {
				Debug.Log(latestLevel + " is more than " + LevelNumber + ", enabling button");
            }
        }
	}
}