using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public GameObject platform;
	
	// Update is called once per frame
	void Update () {
        Debug.Log(Vector3.Distance(gameObject.transform.position, platform.transform.position));
    }
}
