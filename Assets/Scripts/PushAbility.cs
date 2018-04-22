using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PushAbility : NetworkBehaviour {

	//Set move and rotation speed after kicked
	public float moveSpeed = 1000000;
	public float rotationSpeed;

    //Used to control button's behavior
    public static bool pushed = false; 

	private Vector3 targetDir;
    //Reference to a button
    private Button push;
    //Reference to a audioSync script
    private AudioSync audioSync;
    //Reference to a button
    private static GameObject pushButton;
    //Reference to animator
    private Animator animator;		

	//Used to initialize values
	void Start()
    {
		animator = GetComponentInChildren<Animator> ();
		audioSync = GetComponent<AudioSync> ();
	}


    private ColorBlock theColor;

	//Control button's behavior
	public void KickOnClick()
    {

        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        PlayerInfo.UIManager.ResetUI(PlayerInfo.UIManager.kickButton);

        if (!pushed)
        {
            pushed = true;
            GameObject.FindGameObjectWithTag("Kick").GetComponent<Image>().color = Color.clear;
            
        }
        else
        {
			pushed = false;
            GameObject.FindGameObjectWithTag("Kick").GetComponent<Image>().color = Color.white;
        }

       

    }

	//Every tick
	void Update(){

        //Only for local player
        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        if (pushed)
        {
            gameObject.GetComponent<PlayerSkills>().kickTrigger = true;
        }
        else
        {
            gameObject.GetComponent<PlayerSkills>().kickTrigger = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Cmd_CollidersIdentify(this.transform.position, 2.5f, transform.gameObject);
        }


        //if left click and button is active
        if (Input.GetMouseButton (0) && pushed) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {
				//if hitted player
				if (hit.transform.CompareTag ("Player") || hit.transform.CompareTag("DamagedBot")) {
					//Kick across network
					Cmd_CollidersIdentify (this.transform.position, 2.5f, hit.transform.gameObject);

				}
			}
		}

	}

	//Function to AI control
	public void AI_AttackPlayer(GameObject ai_target)
    {

		Cmd_CollidersIdentify(this.transform.position, 3f, ai_target);

	}


	[Command]
	void Cmd_CollidersIdentify(Vector3 center, float radius, GameObject target) {
		//Take all coliders in a radius
         Collider[] hitColliders = Physics.OverlapSphere(center, radius);
         int i = 0;
         while (i < hitColliders.Length) {
			//If player in radius
             if((hitColliders[i].tag =="Player" || hitColliders[i].tag == "DamagedBot") && hitColliders[i].gameObject != gameObject)
                 {
					//then move player across network
					GameObject player = hitColliders[i].gameObject; 
                
                    //Log about kick
                    Logger.LogAction("Kick", gameObject, player);

                    Rpc_Move(player,target);
					return;
                 }
             i++;
             }
         }

	//Move player across network
	[ClientRpc]
	void Rpc_Move(GameObject player, GameObject target){
		//Calculate push vector
		targetDir = target.transform.position - this.transform.position;
		float step = rotationSpeed * Time.deltaTime;

		//Rotate towards a goal
		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);

        //Set animation, sound and deactivate button
       // kickEffect.Play();
        
		animator.SetTrigger ("TriggerAnger");

        //Bug 
		//audioSync.PlaySound (4);

		pushed = false;

        PlayerInfo.UIManager.RepaintKick();

        //Set velocity for target's navmeshAgent
        player.GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();
		player.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity = transform.forward * 15;
	}
}
