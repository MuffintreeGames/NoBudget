using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.CorgiEngine;

public class ContinueDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameSaver saver = GameObject.Find("GameManagers").GetComponent<GameSaver>();
        int latestLevel = saver.GetLatestLevel() + 1;
        if (latestLevel == 1)
        {
            Debug.Log("fresh start");
            this.gameObject.SetActive(false);
            return;
        }

        Debug.Log("current level is " + latestLevel);
        GameObject.Find("Number").GetComponent<Text>().text = latestLevel.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
