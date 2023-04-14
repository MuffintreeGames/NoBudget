using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public int enemyCount;
    // Start is called before the first frame update
    void Start()
    {
        enemyCount = this.transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
