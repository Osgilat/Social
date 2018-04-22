using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShootAbility : NetworkBehaviour {

    //SyncVars for player state
    [SyncVar]
    public bool stunned = false;   
    [SyncVar]
    public bool alive = true;

    //SyncVars for ammo and lighting
    [SyncVar]
	public bool hasAmmo = true; 
	private bool lightIsON = false;

    //Reference to animator
    private Animator animator;		
   

    public GameObject bulletPrefab = null;
    public Transform bulletSpawn = null;
    public float rotationSpeed = 120.0f;

    //bool to check if locked on another player
    public bool locked = false;

    //Particle System above player to indicate who shot
    public ParticleSystem indicator;
    //Light effect for shooting other players
    public GameObject shootLightPrefab;
    //Spawn point for a light effect
    public Transform lightSpawn;			
    public Transform diePoint;

    //used for rotation
    private Quaternion q;
    //player on which this locked
    private GameObject lockedPlayer;
    //reference to this navmeshAgent
    private UnityEngine.AI.NavMeshAgent navmeshAgent;
    //Used to take value on Initiate function 
    private GameObject shootLightLocal;
    //Vector to a target player location
    private Vector3 targetDir;

    //used to shoot bullet in onclick function
    private static bool shootBool = false;

    //time to heal player after he stunned
    public float timeForHeal = 7.0f;

    //to check points
    [SyncVar]
	public int shootPoints = 0;  

    //Used to initialize values
    private void Start()
    {
        navmeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public static bool targeted = false;

    public void TargetOnClick()
    {
        if (!gameObject.GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        PlayerInfo.UIManager.ResetUI(PlayerInfo.UIManager.targetButton);
        if (!targeted)
        {
            targeted = true;
            GameObject.FindGameObjectWithTag("TargetButton").GetComponent<Image>().color = Color.clear;
            
        }
        else
        {
            targeted = false;
            GameObject.FindGameObjectWithTag("TargetButton").GetComponent<Image>().color = Color.white;
        }
    }

	//Called OnClick on SHOOT button
	public void ShootOnClick()
    {
        if (!gameObject.GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        shootBool = true;
    }


    
 
    void Update()
    {
        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer()
            || !UseTeleport.initializeTrigger)
        {
            return;

        }



        if (!hasAmmo)
        {
            PlayerInfo.UIManager.shootButton.SetActive(false);
            PlayerInfo.UIManager.targetButton.SetActive(false);
        }
        else
        {
            PlayerInfo.UIManager.targetButton.SetActive(true);
        }

        if (stunned)
        {
            shootBool = false;
        }

		if (shootBool) {
			Cmd_Shoot ();
		}

        
        //take a mouse position at a moment
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if locked on a player then rotate to the target player location 
        if (locked)
        {
            q = Quaternion.LookRotation(lockedPlayer.transform.position - transform.position);
            transform.position = transform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 200 * Time.deltaTime);
        }

        //if clicked then take raycast
        if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit))
        {
            //if clicked on ground then unlock bool and player and call StopLight
            if (hit.transform.CompareTag("Ground") && hasAmmo)
            	{
                PlayerInfo.UIManager.shootButton.SetActive(false);
                locked = false;
                targeted = false;
                
                GameObject.FindGameObjectWithTag("TargetButton").GetComponent<Image>().color = Color.white;
                
                lockedPlayer = null;
				CmdTurnLightOFF ();
                //else if clicked on player and not locked on anybody then reset path, lock on this player by bool and gameobject
           		 }
            //else if (hit.transform.CompareTag("Player") && !locked && hit.transform.gameObject != gameObject)

            else if (hit.transform.CompareTag("Player") && hit.transform.gameObject != gameObject && targeted)
            {
                targeted = false;
				navmeshAgent.ResetPath ();
                lockedPlayer = hit.transform.gameObject;
                locked = true;
            	}
			}
        //if locked on somebody then take a vector to him and toggle light based on angle 
		if (lockedPlayer != null && hasAmmo)
        {
            // gameObject.GetComponent<UseTeleport>().shootButton.SetActive(true);
            PlayerInfo.UIManager.shootButton.SetActive(true);
            targetDir = lockedPlayer.transform.position - transform.position;
			float angle = Vector3.Angle (targetDir, transform.forward);

            if (angle < 5.0f && !lightIsON 
                && !gameObject.GetComponent<PlayerActor>().lightSpawn.GetComponent<Light>().enabled)
            {
                CmdTurnLightON();
            }
            else if (angle > 5.0f)
            {
                CmdTurnLightOFF();
            }

		}
    }

    [Command]
    void CmdTurnLightON()
    {
        lightSpawn.GetComponent<Light>().enabled = true;
        RpcTurnLightON();
    }

    
    [ClientRpc]
	void RpcTurnLightON()
    {
        lightSpawn.GetComponent<Light>().enabled = true;
    }
    
    [Command]
    void CmdTurnLightOFF()
    {
        lightSpawn.GetComponent<Light>().enabled = false;
        RpcTurnLightOFF();
    }

    [ClientRpc]
	void RpcTurnLightOFF()
    {
        lightSpawn.GetComponent<Light>().enabled = false;
    }

    [Command]
    public void CmdRechargeAmmo()
    {
        hasAmmo = true;
        ParticleSystem.MainModule settings = indicator.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 0, .5f));
        RpcRechargeAmmo();
    }

    [ClientRpc]
    void RpcRechargeAmmo()
    {
        hasAmmo = true;
        ParticleSystem.MainModule settings = indicator.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 0, .5f));
    }

    [Command]
    void Cmd_Shoot()
    {

        if (!hasAmmo)
        {
            return;
        }

        if (lightIsON)
        {
            shootPoints += 1;
        }

        


        this.gameObject.GetComponent<AudioSync>().PlaySound(6);
        GameObject obj = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        // setup bullet component
        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.velocity = transform.forward;

        // spawn on the clients
        NetworkServer.Spawn(obj);

        // destroy after 2 secs
        Destroy(obj, 2.0f);


        //Turn light OFF
        CmdTurnLightOFF();
        //Set indicator to blue
        ParticleSystem.MainModule settings = indicator.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 0, 1, .5f));
        RpcShoot();

    }

    [ClientRpc]
	void RpcShoot()
    {
        if (lockedPlayer != null)
        {

            Logger.LogAction("Shoot", gameObject, lockedPlayer);
        }
        else
        {
            Logger.LogAction("Missed", gameObject, null);
        }

        //Set bools 
        shootBool = false;
        locked = false;
        lockedPlayer = null;
        hasAmmo = false;
        //Turn light OFF
        CmdTurnLightOFF();
        //Set indicator to blue
        ParticleSystem.MainModule settings = indicator.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 0, 1, .5f));

    }


    public IEnumerator PlayOneShot(string paramName)
    {
        animator.SetBool(paramName, true);
        yield return null;
        animator.SetBool(paramName, false);
    }

    public void AI_Shoot()
    {
        Cmd_Shoot();
    }

    public void AI_Target(GameObject playerToShoot)
    {
        locked = true;
        targeted = true;
        lockedPlayer = playerToShoot;
    }





}
