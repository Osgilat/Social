using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotsNotifier : MonoBehaviour {

    public static void Notify(string action, GameObject actor, GameObject target)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<AI_behaviour>().enabled)
                player.GetComponent<AI_behaviour>().ActionListener(action, actor, target);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
