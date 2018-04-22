using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;



public class UseTeleport : NetworkBehaviour
{

    [SyncVar]
    public int timesSaved = 0;

	//References to tower positions 
	public GameObject pos_1;
	public GameObject pos_2;

	//Trigger flag
	public static bool trigger = false;

    //Network entities
	[SyncVar]
	private GameObject objectID;
	private NetworkIdentity objNetId;

	//Reference to audioSync script
	private AudioSync audioSync;


    private GameObject thisTeleport;
    private GameObject otherTeleport;

    //Flags for player's states
    public static bool activateTrigger = false;
    public static bool takeOffTrigger = false; 
	public static bool saveTrigger = false;
	public static bool escapeTrigger = false;

    public static bool initializeTrigger = false;

    public static float agentSpeed;

    //Initialiize variables
    void Start()
    {
		audioSync = GetComponent<AudioSync> ();
        agentSpeed = GetComponent<UnityEngine.AI.NavMeshAgent>().speed;
        Invoke("SetInitializeTrigger", 3f);

    }

    public void SetInitializeTrigger()
    {
        initializeTrigger = true;
    }

    private void HandleKeyboard()
    {
        
        //Handle keyboard
        if (PlayerInfo.UIManager.activateButton.activeSelf && Input.GetKeyDown(KeyCode.Alpha1))
        {
            objectID = otherTeleport;

            CmdActivateOtherTeleport(objectID);
        }

        


        if (PlayerInfo.UIManager.escapeButton.activeSelf && Input.GetMouseButtonDown(1))
        {
           // audioSync.PlayLocalSound(8);

            //For each game manager activate escape button
            foreach (GameObject manager in GameObject.FindGameObjectsWithTag("GameManager"))
            {
                CmdActivateEscapeButton(manager);
            }
        }

        if (PlayerInfo.UIManager.takeOffButton.activeSelf && Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Transform player to first position
            Cmd_TransformPlayer(this.transform.gameObject, pos_1.transform.position, transform.rotation, false);
        }
        else
        if (PlayerInfo.UIManager.saveButton.activeSelf)
        {

            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Transform hitted player
                Cmd_TransformPlayer(PlayerInfo.localPlayerGameObject.GetComponent<PlayerInfo>().playerToTarget1, 
                    pos_2.transform.position, transform.rotation, false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //Transform hitted player
                Cmd_TransformPlayer(PlayerInfo.localPlayerGameObject.GetComponent<PlayerInfo>().playerToTarget2,
                    pos_2.transform.position, transform.rotation, false);
            }

        }
    }

    

    void Update ()
    {
        //Only for local player 



        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;

        }

        if (initializeTrigger)
        {
            HandleKeyboard();
        }
        

        if (saveTrigger)
        {
            gameObject.GetComponent<PlayerSkills>().saveTrigger = true;
        }
        else
        {
            gameObject.GetComponent<PlayerSkills>().saveTrigger = false;
        }

        //if pushed take off
        if (takeOffTrigger && !trigger)
        {
            trigger = true;

            //Transform player to first position
            Cmd_TransformPlayer(gameObject, pos_1.transform.position, pos_1.transform.rotation, false);
        }

        //if save button pushed
        if (saveTrigger)
        {
            //if clicked 
            if (Input.GetMouseButton(0))
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {

                    //if hitted player
                    if (hit.transform.CompareTag("Player")
                        && !hit.transform.gameObject.GetComponent<HybernationSystem>().isHybernated())
                    {
                        //Transform hitted player
                        Cmd_TransformPlayer(hit.transform.gameObject, pos_2.transform.position, pos_2.transform.rotation, false);


                        // TODO: Triggers handling

                        //Reset triggers
                        saveTrigger = false;
                    }
                }
            }
        }

        //if activate button pushed 
        if (activateTrigger)
        {
            //Reset trigger
            activateTrigger = false;

            //Get reference to other Teleport
            objectID = otherTeleport;

            //Activate other teleport
            CmdActivateOtherTeleport(objectID);

        }

        //if escape button pushed 
        if (escapeTrigger)
        {
            //Reset trigger
            escapeTrigger = false;


            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                //for each game manager activate escape button
                foreach (GameObject manager in GameObject.FindGameObjectsWithTag("GameManager"))
                {
                    CmdActivateEscapeButton(manager);
                }
        }
    }








    //Send other teleport  object to a server and then rpc on all the clients
    [Command]
	void CmdActivateOtherTeleport (GameObject obj)
    {
        objNetId = obj.GetComponent<NetworkIdentity> ();
		objNetId.AssignClientAuthority(connectionToClient);
		Rpc_ActivateOtherTeleport (obj);
		objNetId.RemoveClientAuthority(connectionToClient);
	}


	//Rpc other teleport
	[ClientRpc]
	void Rpc_ActivateOtherTeleport(GameObject obj)
    {
		//Get particle system from received object
		ParticleSystem teleport = obj.GetComponent<ParticleSystem> ();

		//if particle system is active then stop and clear
		if (teleport.isPlaying) {

            Logger.LogAction("Deactivated", gameObject, null);

            teleport.Stop ();
			teleport.Clear ();

		
		}
        //if stopped then set active
        else if (teleport.isStopped)
        {
            Logger.LogAction("Activated", gameObject, obj);

            teleport.Play ();
		}
	}

	//Send other teleport  object to a server and then rpc on all the clients
	[Command]
	void CmdStopTeleport (GameObject obj)
    {
		objNetId = obj.GetComponent<NetworkIdentity> ();
		objNetId.AssignClientAuthority (connectionToClient);
		Rpc_StopTeleport (obj);
		objNetId.RemoveClientAuthority (connectionToClient);
	}

	//Deactivate other teleport by stopping and clear particle system
	[ClientRpc]
	void Rpc_StopTeleport(GameObject obj)
    {
		ParticleSystem teleport = obj.GetComponent<ParticleSystem> ();

		if (teleport.isPlaying)
        {
			teleport.Stop ();
			teleport.Clear ();
		}

	}


    public void RespawnPlayers(GameObject obj, Vector3 position, Quaternion rotation, bool roundEnd)
    {

        if (roundEnd && GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            if (GameObject.FindGameObjectWithTag("GameManager")
                   .GetComponent<GameManagerTeleports>().m_TriggerList
                   .Contains(GetComponent<Collider>()))

            {

                audioSync.PlayLocalSound(14);
            }
            else
            {

                audioSync.PlayLocalSound(7);
            }
        }
        /*
        else if (!roundEnd)
        {
            audioSync.PlayLocalSound(9);
        }
        */
        Cmd_TransformPlayer(obj, position, rotation, roundEnd);
    }


	//Transform player on a server
	[Command]
	public void Cmd_TransformPlayer(GameObject obj, Vector3 position, Quaternion rotation, bool roundEnd)
    {
        
        
        



        //Call function on clients
        Rpc_TransformPlayer (obj, position, rotation, roundEnd);
	}

	//Transform player on a clients
	[ClientRpc]
	public void Rpc_TransformPlayer(GameObject obj, Vector3 position, Quaternion rotation, bool roundEnd)
    {

        

        if (GameObject.FindGameObjectWithTag("GameManager")
                    .GetComponent<GameManagerTeleports>().m_TriggerList.Count > 3
                    && !roundEnd
                    || GameManagerTeleports.escaped)
        {
            PlayerInfo.UIManager.DeactivateAllButtons();
            return;
        }

        /*
        if (roundEnd && GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            if (GameObject.FindGameObjectWithTag("GameManager")
                   .GetComponent<GameManagerTeleports>().m_TriggerList
                   .Contains(GetComponent<Collider>()))

            {

                audioSync.PlayLocalSound(14);
            }
            else
            {

                audioSync.PlayLocalSound(7);
            }
        }
        else if (!roundEnd)
        {
            audioSync.PlayLocalSound(9);
        }
        */


        //if not round end then add times saved
        if (!roundEnd)
        {
            audioSync.PlayLocalSound(9);
            obj.GetComponent<UseTeleport>().timesSaved += 1;
            if (position == pos_2.transform.position)
            {
                Logger.LogAction("Saved", gameObject, obj);

                //Deactivate tower buttons
                if (PlayerInfo.localPlayerGameObject == gameObject)
                    PlayerInfo.UIManager.DeactivateAllButtons();
                
            }
            else
            {
                Logger.LogAction("TakeOff", gameObject, null);

                if (PlayerInfo.localPlayerGameObject == gameObject)
                    PlayerInfo.UIManager.ChangeButtonsOnTower();

                //When somebody on tower takeOff to second position
                if (GameObject.FindGameObjectWithTag("GameManager")
                    .GetComponent<GameManagerTeleports>().m_TriggerList.Count > 0)
                {
                    position = pos_2.transform.position;
                }
            }

            obj.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
        }
        
        if (roundEnd)
        {
            //enable navMeshAgent
            obj.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = agentSpeed;
            //Disable announcement text
            PlayerInfo.UIManager.DisableAnnouncementText();
        }

        //Warp to a new position 
        obj.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(position);

        //Disable navMeshAgent
        //obj.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;

        //Set fixed transform rotation
        //obj.transform.eulerAngles = new Vector3(0, 45, 0);

        obj.transform.rotation = rotation;

        

    }

    //Call escape for a player on a server
    [Command]
	void CmdActivateEscapeButton(GameObject obj){
		objNetId = obj.GetComponent<NetworkIdentity> ();
		objNetId.AssignClientAuthority (connectionToClient);
		RpcActivateEscapeButton (obj);
		objNetId.RemoveClientAuthority (connectionToClient);
	}

	//Set escaped trigger to true
	[ClientRpc]
	void RpcActivateEscapeButton(GameObject manager)
    {
        if(!GameManagerTeleports.escaped)
        Logger.LogAction("Escaped", gameObject, null);
        GameManagerTeleports.escaped = true;
	}

	//if player entered collider
	void OnTriggerEnter(Collider teleportInReach)
	{
        if (GetComponent<PlayerInfo>().IsTrueLocalPlayer() || GetComponent<AI_behaviour>().enabled)
        {
            if (teleportInReach.gameObject.tag == "Platform_1" || teleportInReach.gameObject.tag == "Platform_2")
            {
                if (GetComponent<PlayerInfo>().IsTrueLocalPlayer())
                    PlayerInfo.UIManager.SetActivateButton(true);
                //Log information to a server
                Logger.LogAction("EnterTP", gameObject, null);
            }

            //if entered platform 1 then reassign variables
            if (teleportInReach.gameObject.tag == "Platform_1")
            {

                thisTeleport = teleportInReach.gameObject;
                otherTeleport = GameObject.FindGameObjectWithTag("Platform_2");

            }
            //Do same for other teleport
            else if (teleportInReach.gameObject.tag == "Platform_2")
            {

                thisTeleport = teleportInReach.gameObject;
                otherTeleport = GameObject.FindGameObjectWithTag("Platform_1");

            }
        }    
    }

	//When player stay on teleport
	void OnTriggerStay(Collider teleportInReach)
	{
        if (GetComponent<PlayerInfo>().IsTrueLocalPlayer() || GetComponent<AI_behaviour>().enabled)
        {
            if (teleportInReach.gameObject.tag == "Platform_1")
            {

                thisTeleport = teleportInReach.gameObject;
                otherTeleport = GameObject.FindGameObjectWithTag("Platform_2");

                if (thisTeleport.GetComponent<ParticleSystem>().isPlaying && GetComponent<PlayerInfo>().IsTrueLocalPlayer())
                {
                    PlayerInfo.UIManager.SetTakeOffButton(true);
                }
                else if (thisTeleport.GetComponent<ParticleSystem>().isStopped && GetComponent<PlayerInfo>().IsTrueLocalPlayer())
                {
                    PlayerInfo.UIManager.SetTakeOffButton(false);
                }

            }
            else if (teleportInReach.gameObject.tag == "Platform_2")
            {
                thisTeleport = teleportInReach.gameObject;
                otherTeleport = GameObject.FindGameObjectWithTag("Platform_1");

                if (thisTeleport.GetComponent<ParticleSystem>().isPlaying && GetComponent<PlayerInfo>().IsTrueLocalPlayer())
                {
                    PlayerInfo.UIManager.SetTakeOffButton(true);
                }
                else if (thisTeleport.GetComponent<ParticleSystem>().isStopped && GetComponent<PlayerInfo>().IsTrueLocalPlayer())
                {
                    PlayerInfo.UIManager.SetTakeOffButton(false);
                }
            }
            
        }

    }

	//if exited teleport
	void OnTriggerExit(Collider teleportInReach)
	{

        if (GetComponent<PlayerInfo>().IsTrueLocalPlayer() || GetComponent<AI_behaviour>().enabled)
        {
            if (teleportInReach.gameObject.tag == "Platform_1" || teleportInReach.gameObject.tag == "Platform_2")
            {
                //Deactivate teleport buttons 
                if (GetComponent<PlayerInfo>().IsTrueLocalPlayer())
                {
                    PlayerInfo.UIManager.DisableTeleportButtons();
                }

                //Log information about exiting
                Logger.LogAction("exitTP", gameObject, null);

                //Send command to a server to stop teleport 
                objectID = otherTeleport;
                CmdStopTeleport(objectID);

                //Send command to a server to stop teleport 
                objectID = thisTeleport;
                CmdStopTeleport(objectID);

                // TODO: Triggers teleports handling

                //Reset triggers
                thisTeleport = null;
                otherTeleport = null;
            }
        }
    }


	//Triggers button
	public void ActivateTeleportOnClick()
    {

        if(gameObject.GetComponent<PlayerInfo>().IsTrueLocalPlayer())
            activateTrigger = true;

	}

    public void AI_ActivateTeleport()
    {
        objectID = otherTeleport;
        CmdActivateOtherTeleport(objectID);
    }

    public void TakeOffOnClick()
    {
        if (gameObject.GetComponent<PlayerInfo>().IsTrueLocalPlayer())
            takeOffTrigger = true;
    }


    //Used for AI actions triggering
    public void AI_TakeOff()
    {

        if (thisTeleport.GetComponent<ParticleSystem>().isPlaying)
            //|| otherTeleport.GetComponent<ParticleSystem>().isPlaying)
        {
            //Transform player to first position
            Cmd_TransformPlayer(gameObject, pos_1.transform.position, pos_1.transform.rotation, false);
        }
    }



    //Triggers button
    public void SaveOnClick(){

        if (gameObject.GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            PlayerInfo.UIManager.ResetUI(PlayerInfo.UIManager.saveButton);

            if (!saveTrigger)
            {
                saveTrigger = true;
                GameObject.FindGameObjectWithTag("Save").GetComponent<Image>().color = Color.clear;
            }
            else
            {
                saveTrigger = false;
                GameObject.FindGameObjectWithTag("Save").GetComponent<Image>().color = Color.white;
            }
        }
            
    }

    public void AI_Save(GameObject target)
    {
        //Transform hitted player
        Cmd_TransformPlayer(target, pos_2.transform.position, pos_2.transform.rotation, false);


    }


    //Triggers button
    public void EscapeOnClick()
    {

        if (gameObject.GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            escapeTrigger = true;
        }
            
    }



    /*
   
  

    


    /*
    public void AI_Fix()
    {
        CmdFixGenerator(GameObject.FindGameObjectWithTag("Generator"));
    }
    
        

    



    //Used for AI actions triggering
    public void AI_ActivateSaveButton(GameObject playerToSave)
    {

        //Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Saved" + "," + this.gameObject.name + "," + playerToSave.transform.position + "," + playerToSave.name);
        //LogAction("Saved", playerToSave);

        Cmd_TransformPlayer(playerToSave, pos_2.transform.position, pos_2.transform.rotation, false);

    }


    public void AI_Hybernate(GameObject target)
    {
        
        LogAction("Hybernate", target);
        
        //Transform hitted player
        GameObject[] spawnPoints = GetSpawnPoints();

        RespawnPlayers(target.transform.gameObject, spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.position,
            spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.rotation, false);

        //hit.transform.gameObject.GetComponent<HybernationSystem>().
        CmdHybernate(target.transform.gameObject);
    }
*/





}



