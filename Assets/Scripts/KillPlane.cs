using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class KillPlane : MonoBehaviour
{
    protected LevelManager LManager; 
    // Start is called before the first frame update
    void Start()
    {
        LManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        Debug.Log(LManager);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("yee");
           // LManager.KillPlayer();
        }
    }
}
