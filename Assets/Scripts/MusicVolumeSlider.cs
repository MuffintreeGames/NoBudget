using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.CorgiEngine
{
    public class MusicVolumeSlider : MonoBehaviour
    {
        private GameSaver saver;
        private Slider slider;
        // Start is called before the first frame update
        void OnEnable()
        {
            saver = GameObject.Find("GameManagers").GetComponent<GameSaver>();
            slider = this.GetComponent<Slider>();
            slider.value = saver.GetMusicVolume();
        }

        // Update is called once per frame
        void Update()
        {
            saver.SetMusicVolume(slider.value);
        }
    }
}
