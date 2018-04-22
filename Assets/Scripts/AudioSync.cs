using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//Require source to run
[RequireComponent(typeof(AudioSource))]
//Sync audio clips in Unet
public class AudioSync : NetworkBehaviour {

	//Clips to sync
	public AudioClip[] clips;

	//Use gameobject's audio source
	private AudioSource source;

	// Use this for initialization
	void Start () {
		source = this.GetComponent<AudioSource> ();
	}

	//Play clip from array, sending it to a server
	public void PlaySound(int id){
		if (id >= 0 && id < clips.Length) {
			CmdSendServerSoundID (id);
		}
	}

    //Play clip from array locally
    public void PlayLocalSound(int id) {
        if (id >= 0 && id < clips.Length)
        {
            source.PlayOneShot(clips[id]);
        }
    }

	[Command]
	void CmdSendServerSoundID(int id){
		//Play clip on the listeners clients
		RpcSendSoundIDToClients (id);
	}

	[ClientRpc]
	void RpcSendSoundIDToClients(int id){
		//Play clip once per client
		source.PlayOneShot(clips [id]);
	}
}
