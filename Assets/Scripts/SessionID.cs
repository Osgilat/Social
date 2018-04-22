using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SessionID : NetworkBehaviour{

	public int randomNumber;

	void Start(){
		if (isServer) {
			randomNumber = UnityEngine.Random.Range (0, 100000);
		}
	}
		
}
