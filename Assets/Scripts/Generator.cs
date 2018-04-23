using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Generator : NetworkBehaviour
{
    [SyncVar]
    public bool repaired = false;

    [SyncVar]
    public bool active = false;


    [SyncVar]
    public int repairPoints = 2;
    ParticleSystem[] generatorVisuals = null;
    Light spotlight1;
    Light spotlight2;

	// Use this for initialization
	void Start ()
    {
        generatorVisuals = gameObject.GetComponentsInChildren<ParticleSystem>();
        spotlight1 = GameObject.FindGameObjectWithTag("Spotlight1").GetComponent<Light>();
        spotlight2 = GameObject.FindGameObjectWithTag("Spotlight2").GetComponent<Light>();
    }

	void FixGenerator()
    {
        
        foreach (ParticleSystem particleSystem in generatorVisuals)
        {
            particleSystem.Stop();
           
        }
        repaired = true;
    }

    public void RespawnGenerator()
    {
        spotlight1.color = Color.red;
        spotlight2.color = Color.red;
        foreach (ParticleSystem particleSystem in generatorVisuals)
        {
            particleSystem.Play();
            
        }
        repaired = false;
        active = false;
    }
	

    // Update is called once per frame
	void Update ()
    {
        if (repairPoints <= 0)
        {
            FixGenerator();
            spotlight1.color = Color.green;
            spotlight2.color = Color.green;
        } else if (repairPoints == 1)
        {
            spotlight1.color = Color.yellow;
            spotlight2.color = Color.yellow;
        } else
        {
            RespawnGenerator();
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == PlayerInfo.localPlayerGameObject)
        {
            FixAbility.inGeneratorTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerInfo.localPlayerGameObject)
        {
            FixAbility.inGeneratorTrigger = false;
        }
    }
}
