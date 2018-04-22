using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerSpawnerPassengers : NetworkManager {

	public static int chosenCharacter;

	[SerializeField] public GameObject playerA;
	[SerializeField] public GameObject playerB;
	[SerializeField] public GameObject playerC;
    [SerializeField] public GameObject playerD;
    [SerializeField] public GameObject playerDamaged;

    List<GameObject> meshes = new List<GameObject> ();

	List<string> chars = new List<string>(); //Used to store chars for players

	void Start()
	{
		meshes.Add(playerA);
		meshes.Add(playerB);
		meshes.Add(playerC);
        meshes.Add(playerD);

    }

	//subclass for sending network messages
	public class NetworkMessage : MessageBase {
		public int chosenClass;
	}



	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader) {
		NetworkMessage message = extraMessageReader.ReadMessage< NetworkMessage>();
        
		//Store players in a scene in array
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


        if (players.Length == 0)
        {

            int randCharIndex = Random.Range(0, meshes.Count);

            //Used for a ai representation
            GameObject aiPlayer = null;

            Transform spawn = NetworkManager.singleton.GetStartPosition();

            aiPlayer = Instantiate(meshes[randCharIndex], spawn.position, spawn.rotation) as GameObject;
            //aiPlayer.GetComponent<Main> ().enabled = true;
            meshes.RemoveAt(randCharIndex);
            NetworkServer.Spawn(aiPlayer);

            //Used for a ai representation
            aiPlayer = null;

            spawn = NetworkManager.singleton.GetStartPosition();
            randCharIndex = Random.Range(0, meshes.Count);

            aiPlayer = Instantiate(meshes[randCharIndex], spawn.position, spawn.rotation) as GameObject;
            //aiPlayer.GetComponent<Main> ().enabled = true;
            meshes.RemoveAt(randCharIndex);
            NetworkServer.Spawn(aiPlayer);

            randCharIndex = Random.Range(0, meshes.Count);

            //Used for a ai representation
            aiPlayer = null;

         

            aiPlayer = Instantiate(playerDamaged, new Vector3(0,0,0), spawn.rotation) as GameObject;
            //       aiPlayer.GetComponent<Main>().enabled = true;
          //  meshes.RemoveAt(randCharIndex);
            NetworkServer.Spawn(aiPlayer);

            randCharIndex = Random.Range(0, meshes.Count);

            aiPlayer = null;

            spawn = NetworkManager.singleton.GetStartPosition();

            aiPlayer = Instantiate(meshes[randCharIndex], spawn.position, spawn.rotation) as GameObject;
            aiPlayer.GetComponent<Behaviour>().enabled = true;
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

            meshes.RemoveAt(randCharIndex);

            NetworkServer.AddPlayerForConnection(conn, localPlayer, playerControllerId);

        }














        /*
		if (players.Length == 0) {
			Transform spawn = NetworkManager.singleton.GetStartPosition ();
			localPlayer = Instantiate (player, spawn.position, spawn.rotation) as GameObject;


		} else if (players.Length == 1) {
			Transform spawn = NetworkManager.singleton.GetStartPosition ();
			localPlayer = Instantiate (player, spawn.position, spawn.rotation) as GameObject;

		} else if (players.Length == 2) {
			Transform spawn = NetworkManager.singleton.GetStartPosition ();
			localPlayer = Instantiate (player, spawn.position, spawn.rotation) as GameObject;
		}
		*/






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
