using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{

    [System.Serializable]
    public class SaveData
    {
        public int latestLevel;
        public Dictionary<int, float> bestTimes;
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public Dictionary<string, bool> seenTutorials;
        public SaveData()
        {
            latestLevel = 0;
            bestTimes = new Dictionary<int, float>();
            masterVolume = 0.8f;
            musicVolume = 0.8f;
            sfxVolume = 0.8f;
            seenTutorials = new Dictionary<string, bool>();
        }
    }
    public class GameSaver : MonoBehaviour, MMEventListener<PostLevelSaveEvent>
    {
        private SaveData currentSave;
        // Start is called before the first frame update
        void Start()
        {
            LoadGame();
        }

        void Update()
        {
            ApplyOptions();
        }

        void ApplyOptions()
        {
            MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.SetVolumeTrack, MMSoundManager.MMSoundManagerTracks.Master, currentSave.masterVolume);
            MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.SetVolumeTrack, MMSoundManager.MMSoundManagerTracks.Music, currentSave.musicVolume);
            MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.SetVolumeTrack, MMSoundManager.MMSoundManagerTracks.Sfx, currentSave.sfxVolume);
        }

        public void SaveGame()
        {
            MMSaveLoadManager.Save(currentSave, "testName");
        }

        private void LoadGame()
        {
            currentSave = (SaveData)MMSaveLoadManager.Load(System.Type.GetType("SaveData"), "testName");
            if (currentSave == null)
            {
                Debug.Log("making fresh data");
                currentSave = new SaveData();
                SaveGame();
            }
        }

        public void DeleteSave()
        {
            MMSaveLoadManager.DeleteSave("testName");
        }

        public void SetLatestLevel(int newLevel)
        {
            if (newLevel > currentSave.latestLevel)
            {   //shouldn't ever need to revert progress
                currentSave.latestLevel = newLevel;
            }
        }

        public int GetLatestLevel()
        {
            return currentSave.latestLevel;
        }

        public void SetBestTime(int level, float time)
        {
            float oldBestTime = -1f;
            if (!currentSave.bestTimes.TryGetValue(level, out oldBestTime))
            {
                currentSave.bestTimes.Add(level, time);
            }
            else
            {
                if (time < oldBestTime) //best times only go down
                {
                    currentSave.bestTimes[level] = time;
                }
            }
            SaveGame();
        }

        public float GetBestTime(int level)
        {
            float bestTime = -1f;
            if (!currentSave.bestTimes.TryGetValue(level, out bestTime))
            {
                return -1f;
            }
            return currentSave.bestTimes[level];
        }

        public void SetSeenTutorial(string name)
        {
            bool seen;
            if (!currentSave.seenTutorials.TryGetValue(name, out seen))
            {
                currentSave.seenTutorials.Add(name, true);
            }
        }

        public bool GetSeenTutorial(string name)
        {
            bool seen;
            if (!currentSave.seenTutorials.TryGetValue(name, out seen))
            {
                return false;
            }
            return seen;
        }

        public void SetMasterVolume(float volume)
        {
            currentSave.masterVolume = Mathf.Clamp(volume, 0, 1);
        }

        public float GetMasterVolume()
        {
            return currentSave.masterVolume;
        }

        public void SetMusicVolume(float volume)
        {
            currentSave.musicVolume = Mathf.Clamp(volume, 0, 1);
        }

        public float GetMusicVolume()
        {
            return currentSave.musicVolume;
        }

        public void SetSFXVolume(float volume)
        {
            currentSave.sfxVolume = Mathf.Clamp(volume, 0, 1);
        }

        public float GetSFXVolume()
        {
            return currentSave.sfxVolume;
        }

        public virtual void OnMMEvent(PostLevelSaveEvent saveEvent)
        {
            Debug.Log("received event; time for level " + saveEvent.Level + " is " + saveEvent.Time);
            SetLatestLevel(saveEvent.Level + 1);
            if (saveEvent.Time != -1f)  //use -1f for levels that aren't timed
            {
                SetBestTime(saveEvent.Level, saveEvent.Time);
            }
            SaveGame();
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<PostLevelSaveEvent>();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<PostLevelSaveEvent>();
        }
    }
}
