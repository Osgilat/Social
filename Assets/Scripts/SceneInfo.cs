using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

//Used to show info on left top 
public class SceneInfo : NetworkBehaviour {

	[SyncVar]
	public string sessionID;

    public string sceneName = "noSceneName";

    void Start (){
		if (isServer)
        {
            sessionID = UnityEngine.Random.Range(0, 100000).ToString();
            sceneName = SceneManager.GetActiveScene().name;

            switch (sceneName)
            {
                case "Teleports":
                    sessionID = "TP" + sessionID;
                    break;
			case "TeleportsML":
				sessionID = "TPML" + sessionID;
				break;
                case "ThreeShooters":
                    sessionID = "SH" + sessionID;
                    break;
                case "Passengers":
                    sessionID = "PS" + sessionID;
                    break;
                default:
                    sessionID = "WrongScene";
                    break;
            }
        }
        

    }

    

	void OnGUI()
    {

		DateTime time = DateTime.Now;

		//Format date by padding (added 0)
		String hour = time.Hour.ToString().PadLeft (2, '0');
		String minute = time.Minute.ToString().PadLeft (2, '0');
		String second = time.Second.ToString().PadLeft (2, '0');

        GUILayout.Label(hour + ":" + minute + ":" + second
            + " SessionID:  " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID);
	}


}
