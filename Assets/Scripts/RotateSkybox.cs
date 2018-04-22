using UnityEngine;
using System.Collections;

public class RotateSkybox : MonoBehaviour {
	//Used on a camera object with Skybox component

	public float rotation; 
	private Skybox skybox;

	// Use this for initialization
	void Start () {
		skybox = GetComponent<Skybox> ();
	}
	
	// Update is called once per frame
	void Update () {
		rotation -= Time.deltaTime * 2f;
		skybox.material.SetFloat ("_Rotation", rotation);
	}
}
