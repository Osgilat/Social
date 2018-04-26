using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManagerTeleports : NetworkBehaviour {

	public GameObject messageCanvas;
	public static bool escaped = false;			//Flag to represent escaped player
	public float timeLeft = 300.0f;				//time until round ends
    public float timeLeftInGame = 600.0f;

	private GameObject[] players = null;			//References to a players in a scene
	public GameObject[] spawnPoints = null;			//References to a spawn points in a scene
	private WaitForSeconds m_StartWait;         // Used to have a delay while the round starts.
    private WaitForSeconds beforeEndWait;      // Used to have a delay for explosions.
    private WaitForSeconds m_EndWait;           // Used to have a delay while the round or game ends.
	public List<Collider> m_TriggerList = new List<Collider>();	//List of colliders in a GameObject's volume
    private string winner = "";

	private int m_NumRoundsToEnd = 5000;            // The number of rounds a single player has to win to win the game.
	private float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    private float beforeEndDelay = 7f;
    private float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.

    bool initBool = false;
	public int roundNumber;                  // Which round the game is currently on.

	private Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
	private Image m_MessageImage; 				// Reference to the overlay Image to block player's vision

    public float movementSpeed;

    private GameObject generator;
    private GameObject damagedBot;

    static int notHybPlayer = 0;

    //Clips to play locally
    public AudioClip[] clips;

    //Use gameobject's audio source
    private AudioSource source;

    private void Start()
	{
        source = GetComponent<AudioSource>();

        StartCoroutine(WaitForInitializing());

        
        Invoke("Initialization", 2f);
       
	}

    private IEnumerator WaitForInitializing()
    {
        yield return new WaitForSeconds(1);
        if(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>() != null)
        GameObject.FindGameObjectWithTag("MsgText").GetComponent<Text>().text =
            "SESSION_ID: " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID + "\n Press any key to continue..";

    }



    public bool teleportScene = false;
    public bool shootersScene = false;
    public bool passengersScene = false;

    void Initialization()
    {
        

        switch (SceneManager.GetActiveScene().name)
        {
            case "Teleports":
                teleportScene = true;
                break;
            case "ThreeShooters":
                shootersScene = true;
                break;
            case "Passengers":
                passengersScene = true;
                break;
            default:
                
                break;
        }

        //Find players and spawn points in a game
        players = GameObject.FindGameObjectsWithTag("Player");
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");


        // Create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds(m_StartDelay);
        beforeEndWait = new WaitForSeconds(beforeEndDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);


        movementSpeed = players[0].GetComponent<UnityEngine.AI.NavMeshAgent>().speed;

        Time.timeScale = 0;

        if(passengersScene)
        InitializePassengersFields();

        //Getting an image to block gamer's vision
        if (GameObject.FindGameObjectWithTag("MsgImage") != null)
        {
            m_MessageImage = GameObject.FindGameObjectWithTag("MsgImage").GetComponent<Image>();
            m_MessageText = GameObject.FindGameObjectWithTag("MsgText").GetComponent<Text>();
        }


        initBool = true;
        //Start GameLoop
        StartCoroutine(GameLoop());
    }

    private void InitializePassengersFields()
    {
        damagedBot = GameObject.FindGameObjectWithTag("DamagedBot");
        generator = GameObject.FindGameObjectWithTag("Generator");

        foreach (GameObject player in players)
        {
            player.GetComponent<HybernationSystem>().CmdHybernate(player);
        }



        /*
        foreach (GameObject player in players)
        {

            
            if (player.gameObject.GetComponent<NetworkBehaviour>().isLocalPlayer)
            {
                player.GetComponent<UseTeleport>().CmdDisableHybernate(player);
            }
            else
            {
                player.GetComponent<UseTeleport>().CmdHybernate(player);
            }
            
        }
        */


        /*
        players[notHybPlayer].GetComponent<HybernationSystem>().CmdDisableHybernate(players[notHybPlayer]);
        notHybPlayer++;
        if (notHybPlayer > 3)
        { notHybPlayer = 0; }
    */
    }


    //Used to control round time 
    void Update()
    {
        if (!initBool)
        {
            return;
        }


        timeLeft -= Time.deltaTime;
        timeLeftInGame -= Time.deltaTime;

        string minutes = Mathf.Floor(timeLeft / 60).ToString("00");
        string seconds = Mathf.Floor(timeLeft % 60).ToString("00");

        if (timeLeft > 0 && GameObject.FindGameObjectWithTag("RoundTimer") != null)
            GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>().text = minutes + ":" + seconds;

        if (timeLeftInGame < 0)
        {
            Application.Quit();
        }

        

        if(shootersScene)
        HandleShootersEnding();

        if(passengersScene)
        HandlePassengersEnding();

        if (winner != "")
        {
            m_MessageText.text = winner + " WIN!";
            StartCoroutine(WaitBeforeClosingGame());

        }

    }

    private void HandlePassengersEnding()
    {
        if (generator.GetComponent<Generator>().repaired
            && !damagedBot.GetComponent<ShootAbility>().stunned)
        {
            damagedBot.GetComponent<HybernationSystem>().hybernated = false;
            damagedBot.GetComponent<ShootAbility>().stunned = true;
            damagedBot.GetComponentInChildren<Animator>().SetBool("Died", true);
            damagedBot.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
        }




        int playersInHybernation = 0;

        foreach (GameObject player in players)
        {


            if (!player.GetComponent<HybernationSystem>()
                .hybernationSphere.GetComponent<MeshRenderer>().enabled 
                && generator.GetComponent<Generator>().repaired)
            {
                playersInHybernation += 1;
            }
        }

        if (timeLeft < 0 || generator.GetComponent<Generator>().active)
        {
            GameManagerTeleports.escaped = true;
        }
    }

    public int playersWithAmmo = 0;
    public int notStunnedPlayers = 0;

    private void HandleShootersEnding()
    {
        /*
                        foreach (GameObject player in players)
                        {
                            if (player.GetComponent<ShootAbility>().shootPoints >= 50)
                            {
                                string temp = player.name.Remove(11);
                                winner = temp.Replace("Mod_", " ");
                            }
                        }
                        */

        playersWithAmmo = 0;
        notStunnedPlayers = 0;

        foreach (GameObject player in players)
        {

            if (!player.GetComponent<ShootAbility>().stunned && player.GetComponent<ShootAbility>().alive)
            {
                notStunnedPlayers += 1;
            }

            if (player.GetComponent<ShootAbility>().hasAmmo && player.GetComponent<ShootAbility>().alive)
            {
                playersWithAmmo += 1;
            }

        }

        if (timeLeft < 0 || notStunnedPlayers <= 1  ||
           (notStunnedPlayers != 0 && playersWithAmmo == 0))
        {
            escaped = true;
        }

        foreach (GameObject player in players)
        {
            if (player.GetComponent<ShootAbility>().shootPoints >= 50)
            {
                string temp = player.name.Remove(11);
                winner = temp.Replace("Mod_", " ");
            }
        }
    }

    // This is called from start and will run each phase of the game one after another.
    private IEnumerator GameLoop ()
	{
		// Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
		yield return StartCoroutine (RoundStarting ());
        
		// Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
		yield return StartCoroutine (RoundPlaying());

		// Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
		yield return StartCoroutine (RoundEnding());

       
        
            // This code is not run until 'RoundEnding' has finished.  
            if (roundNumber == m_NumRoundsToEnd)
		{
			Application.Quit();
		}
        else
		{
			// Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
			StartCoroutine (GameLoop ());
		}
	}


	private IEnumerator RoundStarting ()
    {
        if (roundNumber == 0)
        {
            while (!Input.anyKey)
            {
                yield return null;
            }
        }
        else
        {
            if (teleportScene)
            {
                PlayerInfo.UIManager.ResetTeleportButtons();
            }
               
            if (shootersScene)
            {
                PlayerInfo.UIManager.ResetShootersButtons();
                ReinitializeShooters();
            }

            if (passengersScene)
            {
                PlayerInfo.UIManager.ResetPassengersButtons();
                ReinitializePassengers();
            }
                
        }

        

        Time.timeScale = 1;

        // Increment the round number and display text showing the players what round it is.
        roundNumber++;


        //Hide Image
        if (m_MessageText != null)
        {
            m_MessageText.text = string.Empty;
            m_MessageImage.CrossFadeAlpha(0, 0.1f, false);
        }

        //Get array of players in scene
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<AI_behaviour>().enabled)
            {
                player.GetComponent<AI_behaviour>().ListenerSession("RoundStart");
            }
        }


        Logger.LogAction("BeginRound" + roundNumber, null, null);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;

    }

    private void ReinitializePassengers()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<ShootAbility>().enabled = true;
            player.GetComponent<HealAbility>().enabled = true;
            player.GetComponent<HybernationSystem>().enabled = true;
            player.GetComponent<WakeUpAbility>().enabled = true;
            player.GetComponent<HybernationSystem>().hybernated = true;
        }
    }

    private void ReinitializeShooters()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<ShootAbility>().enabled = true;
            player.GetComponent<HealAbility>().enabled = true;
        }
    }


    private IEnumerator RoundPlaying ()
	{
		//Set a blocking text to empty during round
        if(m_MessageText != null)
        {
            m_MessageText.text = string.Empty;
            m_MessageImage.CrossFadeAlpha(0, 0.1f, false);

        }

        
        //Playing until 
        while (teleportScene ?
            m_TriggerList.ToArray().Length < 4 && !escaped && timeLeft > 0
            : !escaped && timeLeft > 0)
		{

			// ... return on the next frame.
			yield return null;
		}
        

        //Kurchatov
        /*
        //Playing until 
        while (
            m_TriggerList.ToArray().Length < 4 && !escaped && timeLeft > 0)
        {
            
            // ... return on the next frame.
            yield return null;
        }
        */

    }

	private IEnumerator WaitBeforeClosingGame(){
		yield return new WaitForSeconds (5);
        Application.Quit();
    }

    
    public static GameObject[] Shift(GameObject[] myArray)
    {
        GameObject[] tArray = new GameObject[myArray.Length];
        for (int i = 0; i < myArray.Length; i++)
        {
            if (i < myArray.Length - 1)
                tArray[i] = myArray[i + 1];
            else
                tArray[i] = myArray[0];
        }
        return tArray;
    }

    private IEnumerator RoundEnding ()
    {
        if (teleportScene)
        {
            
            TeleportVisuals();
        }
            

        if (passengersScene)
        {

            StartCoroutine(PassengersVisuals());
        }
            



        //wait time before block vision
        yield return beforeEndWait;
        //	StartCoroutine(WaitBeforeBlockingVision());
        spawnPoints = Shift(spawnPoints);


        for (int i = 0; i < players.Length; i++)
        {

            ReinitializeTriggers(players[i]);


            if (!players[i].GetComponent<ShootAbility>().stunned)
            {
                players[i].GetComponent<ShootAbility>().shootPoints += 1;
            }

            if (players[i].GetComponent<HybernationSystem>()
                .hybernationSphere.GetComponent<MeshRenderer>().enabled)
            {
                players[i].GetComponent<HybernationSystem>().hybernationPoints += 1;
            }

            //stop player
            players[i].GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();

            //deactivate all buttons 
            PlayerInfo.UIManager.DisableAllButtons();

            RepaintIndicator(players[i]);

            //Log player's spawn information 
            Logger.LogAction("Spawned", players[i], null);




            players[i].GetComponent<UseTeleport>().RespawnPlayers(players[i],
                spawnPoints[i].transform.position,
                spawnPoints[i].transform.rotation, true);

            //Shoot logic reset
            players[i].GetComponent<ShootAbility>().hasAmmo = true;
            players[i].GetComponent<ShootAbility>().locked = false;
            players[i].GetComponent<ShootAbility>().stunned = false;
            players[i].GetComponent<ShootAbility>().alive = true;
            players[i].GetComponent<HealAbility>().healOnceBool = false;
            players[i].gameObject.GetComponentInChildren<Animator>().SetBool("Died", false);
            players[i].GetComponent<ShootAbility>().timeForHeal = 7.0f;
        }

        if(teleportScene)
        RestoreVisuals();

        if(passengersScene)
        DamageBotRestore();

        foreach (GameObject player in players)
        {
            if (player.GetComponent<AI_behaviour>().enabled)
            {
                player.GetComponent<AI_behaviour>().ListenerSession("RoundEnd");
            }
        }

        //Reinitialize GameManager's values
        timeLeft = 300.0f;

        //Log about round end
        /*
         Debug.Log(System.DateTime.Now.ToString("M/d/yyyy") + " "
            + System.DateTime.Now.ToString("HH:mm:ss") + ":"
            + System.DateTime.Now.Millisecond + "," + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID 
             + ", ," + "EndRound" + roundNumber);
             */
        Logger.LogAction("EndRound" + roundNumber, null, null);


        m_MessageText.text = "ROUND END!";
        m_MessageImage.CrossFadeAlpha(100, 0.1f, false);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;

    }

    private void DamageBotRestore()
    {
        ParticleSystem.MainModule s = damagedBot.GetComponent<ShootAbility>().indicator.GetComponent<ParticleSystem>().main;
        s.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 0, .5f));

        damagedBot.GetComponent<UseTeleport>().CmdTransformPlayer(damagedBot, new Vector3(0, 0, 0), Quaternion.identity, true);

        damagedBot.GetComponent<ShootAbility>().hasAmmo = true;
        damagedBot.GetComponent<ShootAbility>().locked = false;
        damagedBot.GetComponent<ShootAbility>().stunned = false;
        damagedBot.GetComponent<ShootAbility>().alive = true;
        damagedBot.GetComponent<HealAbility>().healOnceBool = false;
        damagedBot.gameObject.GetComponentInChildren<Animator>().SetBool("Died", false);
        damagedBot.GetComponent<ShootAbility>().timeForHeal = 7.0f;
    }

    public IEnumerator WaitForSphere(float i)
    {
        yield return new WaitForSeconds(i);

        GameObject.FindGameObjectWithTag("SphereEffect").GetComponent<MeshRenderer>().enabled = true;

    }

    public IEnumerator WaitForExplosion(float i)
    {
        yield return new WaitForSeconds(i);

        foreach (var player in players)
        {
            if (!m_TriggerList.Contains(player.GetComponent<Collider>()))
            {
                player.GetComponentInChildren<Animator>().SetBool("Died", true);
                player.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
            }
        }

        foreach (var vfxs in GameObject.FindGameObjectsWithTag("Explosion"))
        {
            foreach (var vfx in vfxs.GetComponentsInChildren<ParticleSystem>())
            {
                vfx.Play();
            }
        }
        source.PlayOneShot(clips[0]);

    }

    private IEnumerator PassengersVisuals()
    {

        if (!generator.GetComponent<Generator>().repaired)
        {

            foreach (var player in players)
            {
                player.GetComponent<HybernationSystem>().hybernated = false;
                player.GetComponent<ShootAbility>().stunned = true;
                player.GetComponentInChildren<Animator>().SetBool("Died", true);
                player.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;

            }

            damagedBot.GetComponent<HybernationSystem>().hybernated = false;
            damagedBot.GetComponent<ShootAbility>().stunned = true;
            damagedBot.GetComponentInChildren<Animator>().SetBool("Died", true);
            damagedBot.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;

            foreach (var vfxs in GameObject.FindGameObjectsWithTag("Explosion"))
            {
                foreach (var vfx in vfxs.GetComponentsInChildren<ParticleSystem>())
                {
                    vfx.Play();
                }
            }



        }
        else
        {


            GameObject.FindGameObjectWithTag("StarShip").GetComponent<StarshipMovement>().speed = 0.4f;
            yield return new WaitForSeconds(0.4f);
            source.PlayOneShot(clips[2]);
            yield return new WaitForSeconds(1.4f);

            foreach (var player in players)
            {
                if (!player.GetComponent<HybernationSystem>().isHybernated())
                {
                    player.GetComponent<HybernationSystem>().hybernated = false;
                    player.GetComponent<ShootAbility>().stunned = true;
                    player.GetComponentInChildren<Animator>().SetBool("Died", true);
                    player.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
                }
                else
                {
                    player.transform.position = GameObject.FindGameObjectWithTag("DiePosition").transform.position;
                }
            }

            damagedBot.GetComponent<HybernationSystem>().hybernated = false;
            damagedBot.GetComponent<ShootAbility>().stunned = true;
            damagedBot.GetComponentInChildren<Animator>().SetBool("Died", true);
            damagedBot.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;

            GameObject.FindGameObjectWithTag("Radiation").GetComponent<ParticleSystem>().Play();


            source.PlayOneShot(clips[1]);
        }

        /*
        foreach (GameObject player in players)
        {
            player.GetComponent<HybernationSystem>().CmdHybernate(player);
        }
        */

        /*
        players[notHybPlayer].GetComponent<HybernationSystem>().CmdDisableHybernate(players[notHybPlayer]);
        notHybPlayer++;
        if (notHybPlayer > 3) { notHybPlayer = 0; }
        */

        generator.GetComponent<Generator>().repaired = false;
        generator.GetComponent<Generator>().repairPoints = 2;
        generator.GetComponent<Generator>().active = false;
    }

    private void TeleportVisuals()
    {

        StartCoroutine(WaitForSphere(1.0f));



        StartCoroutine(WaitForExplosion(2.0f));

        
    }

    private void RestoreVisuals()
    {
        //for each player in a scene
        foreach (GameObject player in players)
        {

            //Reinitialize speed
            player.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = movementSpeed;
            player.GetComponentInChildren<Animator>().SetBool("Died", false);

        }


        GameObject.FindGameObjectWithTag("SphereEffect").GetComponent<MeshRenderer>().enabled = false;
    }

    

    private static void RepaintIndicator(GameObject player)
    {
        ParticleSystem.MainModule settings = player.GetComponent<ShootAbility>().indicator.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 0, .5f));
    }

    private static void ReinitializeTriggers(GameObject player)
    {
        UseTeleport.takeOffTrigger = false;
        UseTeleport.saveTrigger = false;
        UseTeleport.escapeTrigger = false;
        UseTeleport.trigger = false;
        FixAbility.hasFixCharge = true;
        escaped = false;
    }

    //Used by GameManager's volume to count saved players
    void OnTriggerEnter(Collider other){
		
		//Adding player to a triggerList
		if(other.CompareTag("Player") && !m_TriggerList.Contains(other)){
			m_TriggerList.Add (other);
		}
       
		//Block buttons if there are 2 saved players
        if(teleportScene)
		if (m_TriggerList.ToArray ().Length > 3) {
            PlayerInfo.UIManager.DisableAllButtons();
		}


	}

	//Remove player from triggerList when not in a volume
	void OnTriggerExit(Collider other){

		if(other.CompareTag("Player") && m_TriggerList.Contains(other)){
			m_TriggerList.Remove (other);
		}

    }
    
}
