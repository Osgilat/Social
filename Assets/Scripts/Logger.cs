using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SiSubs;

public class Logger : MonoBehaviour {

    public static void LogAction(String action, GameObject actor, GameObject target)
    {

        
        if (GameObject.FindGameObjectWithTag("MainCamera")
            .GetComponent<SceneInfo>() != null)
        {
            Debug.Log(
                DateTime.Now.ToString("M/d/yyyy") + " "
                + System.DateTime.Now.ToString("HH:mm:ss") + ":"
                + System.DateTime.Now.Millisecond + "," +
                GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<SceneInfo>().sessionID
                + String.Format(",{0},{1},{2},{3}",

                (actor == null || !actor.gameObject.CompareTag("Player")) ?
                "" : actor.GetComponent<PlayerInfo>().playerID.Replace("Player ", ""),

                action,

                (actor == null || !actor.gameObject.CompareTag("Player")) ?
                "" : actor.transform.position.ToString().Replace("(", "").Replace(")", ""),

                (target == null || !target.gameObject.CompareTag("Player")) ?
                    (action == "Move") ?
                    actor.gameObject.GetComponent<PlayerActor>().hit.point.ToString().Replace("(", "").Replace(")", "") : ""
                : target.gameObject.GetComponent<PlayerInfo>().playerID.Replace("Player ", "")));


            Lpt.Send();

        //Notify bots about action
            BotsNotifier.Notify(action, actor, target);
        }
        
    }
}
