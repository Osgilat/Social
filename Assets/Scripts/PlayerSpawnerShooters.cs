using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerSpawnerShooters : NetworkManager {

	public static int chosenCharacter;

	[SerializeField] public GameObject playerA;
	[SerializeField] public GameObject playerB;
	[SerializeField] public GameObject playerC;
   
    List<GameObject> meshes = new List<GameObject> ();

    //Used to store chars for players
    List<string> chars = new List<string>(); 

	void Start()
	{
		meshes.Add(playerA);
		meshes.Add(playerB);
		meshes.Add(playerC);
    }

	//subclass for sending network messages
	public class NetworkMessage : MessageBase {
		public int chosenClass;
	}



	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader) {
		NetworkMessage message = extraMessageReader.ReadMessage< NetworkMessage>();
        
		//Store players in a scene in array
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            player.GetComponent<HybernationSystem>().hybernated = false;
        }

        if (players.Length == 0)
        {
            
            int randCharIndex = Random.Range(0, meshes.Count);

            //Used for a ai representation
            GameObject aiPlayer = null;

            Transform spawn = NetworkManager.singleton.GetStartPosition();

            aiPlayer = Instantiate(meshes[randCharIndex], spawn.position, spawn.rotation) as GameObject;
            aiPlayer.GetComponent<AI_behaviour>().enabled = true;
            aiPlayer.GetComponent<AI_behaviour>().useMoralScheme = true;
            meshes.RemoveAt(randCharIndex);
            NetworkServer.Spawn(aiPlayer);

            //Used for a ai representation
            aiPlayer = null;

            spawn = NetworkManager.singleton.GetStartPosition();

            randCharIndex = Random.Range(0, meshes.Count);

            aiPlayer = Instantiate(meshes[randCharIndex], spawn.position, spawn.rotation) as GameObject;
            aiPlayer.GetComponent<AI_behaviour>().enabled = true;
            aiPlayer.GetComponent<AI_behaviour>().useMoralScheme = false;
            meshes.RemoveAt(randCharIndex);
            NetworkServer.Spawn(aiPlayer);

            
        }



        
        if (meshes.Count != 0)
        {

            int randCharIndex = Random.Range(0, meshes.Count);

            //Used for a local player representation
            GameObject localPlayer = null;


            Transform spawn = NetworkManager.singleton.GetStartPosition();

            localPlayer = Instantiate(meshes[randCharIndex], spawn.position, spawn.rotation) as GameObject;

            //localPlayer.GetComponent<AI_behaviour>().enabled = true;

            meshes.RemoveAt(randCharIndex);

            NetworkServer.AddPlayerForConnection(conn, localPlayer, playerControllerId);

        }

    }

	public override void OnClientConnect(NetworkConnection conn) {
		NetworkMessage test = new NetworkMessage();
		test.chosenClass = chosenCharacter;
		chosenCharacter += 1;
		ClientScene.AddPlayer(conn, 0, test);
	}


	public override void OnClientSceneChanged(NetworkConnection conn) {
		//base.OnClientSceneChanged(conn);
	}
}
