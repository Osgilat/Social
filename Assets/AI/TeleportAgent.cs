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

        AddVectorObs(gameObject.transform.position.x / 10);
        AddVectorObs(gameObject.transform.position.z / 10);

        AddVectorObs(GetComponent<PlayerInfo>().playerToTarget1.transform.position.x / 10);
        AddVectorObs(GetComponent<PlayerInfo>().playerToTarget1.transform.position.z / 10);

        AddVectorObs(GetComponent<PlayerInfo>().playerToTarget2.transform.position.x / 10);
        AddVectorObs(GetComponent<PlayerInfo>().playerToTarget2.transform.position.z / 10);

        AddVectorObs(firstPlatform.GetComponent<ParticleSystem>().isPlaying ? 1 : 0);
        AddVectorObs(secondPlatform.GetComponent<ParticleSystem>().isPlaying ? 1 : 0);
        
    }

    public override void AgentReset()
    {

        /*
       gameObject.transform.position = GameObject.FindGameObjectsWithTag("SpawnPoint")[Random.Range(0,3)]
            .transform.position;
        */
    }

    private float previousDistance = float.MaxValue;
    GameObject memTeleport = null;

    public override void AgentAction(float[] vectorAction, string temp = "")
    {
        firstPlatform = GameObject.FindGameObjectWithTag("Platform_1");
        secondPlatform = GameObject.FindGameObjectWithTag("Platform_2");


        GameObject activePlatform = null;

        if (firstPlatform.GetComponent<ParticleSystem>().isPlaying)
        {
            
            if(secondPlatform.GetComponent<ParticleSystem>().isPlaying)
            {
                if(Vector3.Distance(transform.position,firstPlatform.transform.position) >
                                            Vector3.Distance(transform.position, secondPlatform.transform.position))
                {
                    activePlatform = secondPlatform;
                }
                else
                {
                    activePlatform = firstPlatform;
                }
            }
            else
            {
                activePlatform = firstPlatform;
            }
        }
        else if (secondPlatform.GetComponent<ParticleSystem>().isPlaying)
        {
            activePlatform = secondPlatform;
        }

        

        if (activePlatform == null)
        {
            return;
        }

        

        if (memTeleport != activePlatform)
        {
            previousDistance = float.MinValue;
            memTeleport = activePlatform;
        }
            


        int action = Mathf.FloorToInt(vectorAction[0]);

        Vector3 targetPos = transform.position;

       
        switch (action)
        {

            case 0:
                AddReward(-0.05f);
                targetPos = transform.position + new Vector3(0.05f, 0, 0f);
               
                break;
            case 1:
                AddReward(-0.05f);
                targetPos = transform.position + new Vector3(-0.05f, 0, 0f);
                
                break;
            case 2:
                AddReward(-0.05f);
                targetPos = transform.position + new Vector3(0f, 0, 0.05f);
                
                break;
            case 3:
                AddReward(-0.05f);
                targetPos = transform.position + new Vector3(0f, 0, -0.05f);
                break;

        }

       

        transform.position = targetPos;

        
        float distanceToTarget = Vector3.Distance(transform.position,
                                            activePlatform.transform.position);


        if (Vector3.Distance(gameObject.transform.position, scene.transform.position) > 8f)
        {
            AddReward(-1f);
            Done();
            
        }

       // Debug.Log(previousDistance + " " + distanceToTarget);

        if (distanceToTarget < previousDistance)
        {
            AddReward(0.1f);
        }

        /*
        if (Vector3.Distance(gameObject.transform.position, activePlatform.transform.position) > 5f)
        {
            AddReward(-0.005f);
        }
        
      
        if (Vector3.Distance(gameObject.transform.position, activePlatform.transform.position) < 5f)
        {
            AddReward(0.005f);
        }
        */

        if (distanceToTarget < 2.25f)
        {
            AddReward(1f);
            //gameObject.GetComponent<UseTeleport>().AI_TakeOff();
            //gameObject.GetComponent<UseTeleport>().CmdActivateEscapeButton("Escaped");
            Done();
        }

        previousDistance = distanceToTarget;



    }

}

