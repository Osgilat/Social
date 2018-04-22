using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerActor : NetworkBehaviour
{

    [SyncVar]
    public float speed; //Control speed of player
	[SyncVar]
    public float rotationSpeed; // Control rotation speed of player

	//public GameObject greetLightPrefab; //Light effect for greeting other players
	public Transform lightSpawn;		//Spawn point for a light effect
	public float waitBeforeEffect = .1f; //Delay after greeting to shoot a light effect
	public float lightTime = 0.6f;


	private	UnityEngine.AI.NavMeshAgent navmeshAgent; //Reference to player's navmesh agent
	private Animator animator;						//Reference to an Animator component
	private AudioSync audioSync;					//Reference to player's audioSync script

	//used to toggle greet button active state
	private static bool sayHelpMe = false;

    private static bool sayThank = false;


    public static List<GameObject> players;




    private bool mLock;
    

    //Initialize on start
    void Start()
    {

        navmeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		animator = GetComponentInChildren<Animator> ();
		audioSync = GetComponent<AudioSync> ();
		
		/*
		GameObject.FindGameObjectWithTag ("Greet").GetComponentInChildren<Text> ().text = "ASK";
        GameObject.FindGameObjectWithTag("ThankButton").GetComponentInChildren<Text>().text = "THANK";
        */
    }

    //Used to control button's behavior
    public void AskOnClick()
    {
        
        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        PlayerInfo.UIManager.ResetUI(PlayerInfo.UIManager.askButton);

        if (!sayHelpMe)
        {
            sayHelpMe = true;
            PlayerInfo.UIManager.askButton.GetComponent<Image>().color = Color.clear;
            

        }
        else
        {
            sayHelpMe = false;
            PlayerInfo.UIManager.askButton.GetComponent<Image>().color = Color.white;
        }

    }

    //Used to control button's behavior
    public void OnThankClick()
    {

        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        PlayerInfo.UIManager.ResetUI(PlayerInfo.UIManager.thankButton);
        
        if (!sayThank)
        {
            sayThank = true;
            PlayerInfo.UIManager.thankButton.GetComponent<Image>().color = Color.clear;
            
        }
        else
        {
            sayThank = false;
            PlayerInfo.UIManager.thankButton.GetComponent<Image>().color = Color.white;
        }
        
    }

    //Clicked place
    public RaycastHit hit;


    void FixedUpdate()
    {
        //Control only by local player

        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer() )
        {
            return;
        }

        if (sayHelpMe)
        {
            gameObject.GetComponent<PlayerSkills>().askTrigger = true;
        }
        else
        {
            gameObject.GetComponent<PlayerSkills>().askTrigger = false;
        }

        if (sayThank)
        {
            gameObject.GetComponent<PlayerSkills>().thankTrigger = true;
        }
        else
        {
            gameObject.GetComponent<PlayerSkills>().thankTrigger = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mLock = false;
        }

        //if left clicked
        if (!mLock && Input.GetMouseButtonDown(0)) {

            mLock = true;
           

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
                
				//if clicked on a ground or one of the teleports		
				if (hit.transform.CompareTag ("Ground") || hit.transform.CompareTag ("Platform_1")
				    || hit.transform.CompareTag ("Platform_2")) {

                    CmdLogMove();

                    //Set destination for agent
                    navmeshAgent.destination = hit.point;
				} 

				//if greetButton active and clicked on a player
				if((sayHelpMe || sayThank) && hit.transform.CompareTag ("Player") && hit.transform.gameObject != gameObject)
                {
                    SaySomething(hit.transform.gameObject, sayHelpMe);
                }
                
            }
		}
    }

    //Flicker Light for a server and clients
    [Command]
    void CmdLogMove()
    {

        RpcLogMove();

    }

    //Flicker Light for a server and clients
    [ClientRpc]
    void RpcLogMove()
    {
        //Log movement information
        Logger.LogAction("Move", gameObject, null);
    }


    /// <summary>
    /// Method to activate  
    /// </summary>
    /// <param name="target"></param>
    /// <param name="whatToSay">true = ask, false = thank</param>
    private void SaySomething(GameObject target, bool whatToSay)
    {
        //Calculate vector3 to a clicked player
        Vector3 newDir = Vector3.RotateTowards(transform.forward, target.transform.position - transform.position
            , 1000 * Time.deltaTime, 0.0F);

        //Rotate player
        transform.rotation = Quaternion.LookRotation(newDir);

        
        //Trigger animation
        animator.SetTrigger("SayHello");

        //Flicker effect to a server and clients
        if (whatToSay)
        {
            CmdFlickerLight("ASK");
        }
        else
        {
            CmdFlickerLight("THANK");
        }
        

        //Bunch of ifs to control who and how greet
        if (target.transform.name == "PlayerMod_A(Clone)" && this.name != "PlayerMod_A(Clone)")
        {
            
            if (whatToSay)
            {
                audioSync.PlaySound(0);
            }
            else
            {
                audioSync.PlaySound(10);
            }
        }
        else if (target.transform.name == "PlayerMod_B(Clone)" && this.name != "PlayerMod_B(Clone)")
        {
            
            if (whatToSay)
            {
                audioSync.PlaySound(1);
            }
            else
            {
                audioSync.PlaySound(11);
            }

        }
        else if (target.transform.name == "PlayerMod_C(Clone)" && this.name != "PlayerMod_C(Clone)")
        {
            
            if (whatToSay)
            {
                audioSync.PlaySound(2);
            }
            else
            {
                audioSync.PlaySound(12); 
            }

        }
        else if (target.transform.name == "PlayerMod_D(Clone)" && this.name != "PlayerMod_D(Clone)")
        {
            

            if (whatToSay)
            {
                audioSync.PlaySound(3); 
            }
            else
            {
                audioSync.PlaySound(13);
            }


        }

        if (whatToSay)
        {
            Logger.LogAction("Ask", gameObject, target);

            if (GetComponent<PlayerInfo>().IsTrueLocalPlayer())
            {
                //Toggle button to inactive state
                sayHelpMe = false;
                GameObject.FindGameObjectWithTag("Greet").GetComponent<Image>().color = Color.white;
            }
            
        }
        else
        { 
            Logger.LogAction("Thank", gameObject, target);

            if (GetComponent<PlayerInfo>().IsTrueLocalPlayer())
            {
                //Toggle button to inactive state
                sayThank = false;
                GameObject.FindGameObjectWithTag("ThankButton").GetComponent<Image>().color = Color.white;
            }
            
        }
        

        Cmd_Hello(gameObject, target);

        
    }

    //Wait for slow rotation
    IEnumerator WaitBeforeLightning(string type){
        lightSpawn.GetComponent<Light>().enabled = true;
        if (type == "THANK")
        {
            lightSpawn.GetComponent<Light>().color = new Color(0, 255, 0);
        }
        else if (type == "ASK")
        {
            lightSpawn.GetComponent<Light>().color = new Color(0, 0, 255);
        }
       
        yield return new WaitForSeconds (waitBeforeEffect);
        lightSpawn.GetComponent<Light>().enabled = false;
    }

	//Flicker Light for a server and clients
	[Command]
	void CmdFlickerLight(string type)
    {
       
        StartCoroutine(WaitBeforeLightning(type));
        RpcFlickerLight(type);
        
    }

    //Flicker Light for a server and clients
    [ClientRpc]
    void RpcFlickerLight(string type)
    {
       
        StartCoroutine(WaitBeforeLightning(type));

    }



    [Command]
	void Cmd_Hello(GameObject actor, GameObject target)
    {
        //Calling on clients
		Rpc_Hello (actor, target);
	}

	//For each client make event active
	[ClientRpc]
	void Rpc_Hello(GameObject actor, GameObject target)
    {
        //TODO: Notify bots about hello action
    }

    //TODO: Decouple AI CODE  
	//Used by AI to greet other players
	public void AI_Ask(GameObject ai_target){
        SaySomething(ai_target, true);
        /*
		Transform player = ai_target.transform;
		Vector3 targetDir = player.position - this.transform.position;
		float step = rotationSpeed * Time.deltaTime;

		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0F);
		
		transform.rotation = Quaternion.LookRotation (newDir);

		animator.SetTrigger("SayHello");
        CmdFlickerLight("ASK");

        if (player.name == "PlayerMod_A(Clone)" && this.name != "PlayerMod_A(Clone)") {
           

            audioSync.PlaySound (0);
            
        } else if (player.name == "PlayerMod_B(Clone)" && this.name != "PlayerMod_B(Clone)") {
			audioSync.PlaySound (1);
            
        } else if (player.name == "PlayerMod_C(Clone)" && this.name != "PlayerMod_C(Clone)") {
			audioSync.PlaySound (2);
            
        }

        Logger.LogAction("Ask", gameObject, ai_target);
        Cmd_Hello(this.gameObject, ai_target);
        sayHelpMe = false;
        */

	}

    public void AI_Thank(GameObject ai_target)
    {
        SaySomething(ai_target, false);
        /*
        Transform player = ai_target.transform;
        Vector3 targetDir = player.position - this.transform.position;
        float step = rotationSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);

        animator.SetTrigger("SayHello");
        CmdFlickerLight("THANK");

        if (player.name == "PlayerMod_A(Clone)" && this.name != "PlayerMod_A(Clone)")
        {
            audioSync.PlaySound(8);
            
        }
        else if (player.name == "PlayerMod_B(Clone)" && this.name != "PlayerMod_B(Clone)")
        {
            audioSync.PlaySound(9);

        }
        else if (player.name == "PlayerMod_C(Clone)" && this.name != "PlayerMod_C(Clone)")
        {
            audioSync.PlaySound(10);

        }

        Logger.LogAction("Thank", gameObject, ai_target);
        Cmd_Hello(this.gameObject, ai_target);
        sayHelpMe = false;
        */

    }

}