using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MoreMountains.CorgiEngine;

public class BestTimeDisplay : MonoBehaviour
{
    private GameSaver saver;
    private Text timeDisplay;
    private Text boilerPlate;
    // Start is called before the first frame update
    void Start()
    {
        saver = GameObject.Find("GameManagers").GetComponent<GameSaver>();
        timeDisplay = this.GetComponent<Text>();
        boilerPlate = GameObject.Find("Boilerplate").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject highlightedObject = EventSystem.current.currentSelectedGameObject;
        timeDisplay.enabled = false;
        boilerPlate.enabled = false;
        if (highlightedObject == null)
        {
            return;
        }

        GameObject parentObject = highlightedObject.transform.parent.gameObject.transform.parent.gameObject;
        if (parentObject == null)
        {
            return;
        }

        LevelNumberSelector selector = parentObject.GetComponent<LevelNumberSelector>();
        if (selector == null)
        {
            return;
        }

        timeDisplay.enabled = true;
        boilerPlate.enabled = true;

        float bestTime = saver.GetBestTime(selector.LevelNumber);
        if (bestTime == -1f)
        {
            timeDisplay.text = "N/A";
            return;
        }

        timeDisplay.text = bestTime.ToString("F2");
        Debug.Log("succeeded! " + selector.LevelNumber);
        }
    }
