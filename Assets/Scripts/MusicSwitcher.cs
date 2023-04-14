using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoBudget;

public class MusicSwitcher : MonoBehaviour
{
    public string MusicChoice;

    // Start is called before the first frame update
    void Start()
    {
        MusicPlayerEvent.Trigger(MusicChoice);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
