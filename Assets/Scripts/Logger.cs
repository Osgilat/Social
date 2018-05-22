using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SiSubs;

public class Logger : MonoBehaviour {

    public static readonly int[,] i =
        {       //A  B  C  
      /*ASK 0*/   { 1, 2, 3 }, /*T*/
      /*THANK 1*/ { 4, 5, 6 }, /*T*/
      /*KICK 2*/  { 7, 8, 9 }, /*T*/
  /*ACTIVATE 3*/  { 10, 11, 12 },
   /*TAKEOFF 4*/  { 13, 14, 15 },
      /*SAVE 5*/  { 16, 17, 18 }, /*T*/
    /*ESCAPE 6*/  { 19, 20, 21 },
      /*MOVE 7*/  { 22, 23, 24 },
        /*CH 8*/  { 25, 26, 27 }, /*T*/

        };

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

            

            int match = -1;

            switch (action)
            {
                case "Ask":
                    match = 1;
                    break;
                case "Thank":
                    match = 4;
                    break;
                case "Kick":
                    match = 7;
                    break;
                case "Activated":
                    match = 10;
                    break;
                case "TakeOff":
                    match = 13;
                    break;
                case "Saved":
                    match = 16;
                    break;
                case "Escaped":
                    match = 19;
                    break;
                case "Move":
                    match = 22;
                    break;
                case "ConfirmedHuman":
                    match = 25;
                    break;

                default:
                    match = -1;
                    break;
            }

            int targetMatch = -1;

            if(target != null)
            {
                switch (target.GetComponent<PlayerInfo>().playerID)
                {
                    case "Player A":
                        targetMatch = match + 0;
                        break;
                    case "Player B":
                        targetMatch = match + 1;
                        break;
                    case "Player C":
                        targetMatch = match + 2;
                        break;
                }
            }
            

            switch (actor.GetComponent<PlayerInfo>().playerID)
            {
                case "Player A":
                    match += 0;
                    break;
                case "Player B":
                    match += 1;
                    break;
                case "Player C":
                    match += 2;
                    break;
            }

           

            Debug.Log("MATCH " + match + " TARGET " + targetMatch);
            if (match != (-1))
            {
                if(targetMatch != -1)
                {
                    Lpt.Send(match, targetMatch);
                }
                else
                {
                    Lpt.Send(match, match);
                }
            }
            

        //Notify bots about action
            BotsNotifier.Notify(action, actor, target);
        }
        
    }
}
