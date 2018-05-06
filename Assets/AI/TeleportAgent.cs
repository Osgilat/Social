using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportAgent : Agent
{

    
    public GameObject firstPlatform;
    public GameObject secondPlatform;
    public GameObject scene;


    public override void CollectObservations()
    { 
        
        AddVectorObs(gameObject.transform.position);
        
        /*
        AddVectorObs(firstPlatform.GetComponent<ParticleSystem>().isPlaying ? 1 : 0);
        AddVectorObs(secondPlatform.GetComponent<ParticleSystem>().isPlaying ? 1 : 0);
        */
    }

    public override void AgentReset()
    {


       gameObject.transform.position = GameObject.FindGameObjectsWithTag("SpawnPoint")[Random.Range(0,3)]
            .transform.position;
        
    }



    public override void AgentAction(float[] vectorAction, string temp = "")
    {
        AddReward(-0.01f);

        int action = Mathf.FloorToInt(vectorAction[0]);

        Vector3 targetPos = transform.position;
        
        switch (action)
        {

            case 0:
                
                targetPos = transform.position + new Vector3(0.1f, 0, 0f);
               
                break;
            case 1:
                
                targetPos = transform.position + new Vector3(-0.1f, 0, 0f);
                
                break;
            case 2:
                targetPos = transform.position + new Vector3(0f, 0, 0.1f);
                
                break;
            case 3:
                
                targetPos = transform.position + new Vector3(0f, 0, -0.1f);
                break;

        }
        

        if (Vector3.Distance(gameObject.transform.position, scene.transform.position) > 8f)
        {
            AddReward(-1f);
            Done();
            
        }

        
        if (Vector3.Distance(gameObject.transform.position, secondPlatform.transform.position) > 5f)
        {
            AddReward(-0.005f);
        }
        
      
        if (Vector3.Distance(gameObject.transform.position, secondPlatform.transform.position) < 5f)
        {
            AddReward(0.005f);
        }
        

        if (Vector3.Distance(gameObject.transform.position, secondPlatform.transform.position) < 3f)
        {
            AddReward(1f);
            //gameObject.GetComponent<UseTeleport>().AI_TakeOff();
            //gameObject.GetComponent<UseTeleport>().CmdActivateEscapeButton("Escaped");
            Done();
        }
            transform.position = targetPos;

           

    }

}

