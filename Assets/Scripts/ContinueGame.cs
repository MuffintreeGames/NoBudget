using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	public class ContinueGame : MonoBehaviour
	{
		/// <summary>
		/// Loads the level specified in the inspector
		/// </summary>
		public virtual void Continue()
		{
			GameSaver saver = GameObject.Find("GameManagers").GetComponent<GameSaver>();
			int latestLevel = saver.GetLatestLevel();
			if (latestLevel >= LevelNumberMapper.levelMap.Length) {
				Debug.Log("invalid level number of " + latestLevel);
				return;
			}
			string levelName = LevelNumberMapper.levelMap[latestLevel];
			Debug.Log(levelName);
			LevelManager.Instance.GotoLevel(levelName, true, false);
		}
	}
}