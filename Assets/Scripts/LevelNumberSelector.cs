using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// This component allows the definition of a level that can then be accessed and loaded. Used mostly in the level map scene.
	/// </summary>
	public class LevelNumberSelector : MonoBehaviour
	{
		/// the exact name of the target level
		[Tooltip("the exact name of the target level")]
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


		/// <summary>
		/// Loads the level specified in the inspector
		/// </summary>
		public virtual void GoToLevel()
		{
			string levelName = LevelNumberMapper.levelMap[LevelNumber];
			LevelManager.Instance.GotoLevel(levelName, true, false);
		}
	}
}