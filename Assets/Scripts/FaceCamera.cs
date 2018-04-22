using UnityEngine;
using System.Collections;

//Used to face chars above players heads to a camera script attached to
public class FaceCamera : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		//Take a vector to a camera 
       this.transform.LookAt(Camera.main.transform.position);
		//Keeps same rotation
       this.transform.Rotate(new Vector3(0, 180, 0));	
	}

}
