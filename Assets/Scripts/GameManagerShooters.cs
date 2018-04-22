using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManagerShooters : NetworkBehaviour {
    /*
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



    static int notHybPlayer = 0;

    //Clips to play locally
    public AudioClip[] clips;

    //Use gameobject's audio source
    private AudioSource source;

    private void Start()
	{
        source = this.GetComponent<AudioSource>();

        StartCoroutine(WaitForInitializing());
        
        Invoke("Initialization", 2f);
       
	}

    private IEnumerator WaitForInitializing()
    {
        yield return new WaitForSeconds(1);
        GameObject.FindGameObjectWithTag("MsgText").GetComponent<Text>().text =
            "SESSION_ID: " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID + "\n Press any key to continue..";

    }





    void Initialization()
    {
        //Find players and spawn points in a game
        players = GameObject.FindGameObjectsWithTag("Player");
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
  

        // Create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds(m_StartDelay);
        beforeEndWait = new WaitForSeconds(beforeEndDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

  

        Time.timeScale = 0;

      
        //Getting an image to block gamer's vision
        m_MessageImage = GameObject.FindGameObjectWithTag("MsgImage").GetComponent<Image>();
        m_MessageText = GameObject.FindGameObjectWithTag("MsgText").GetComponent<Text>();

      

        initBool = true;
        //Start GameLoop
        StartCoroutine(GameLoop());
    }
    
    
        //Used to control round time 
        void Update()
	{
        if (!initBool){
            return;
        }

      
		timeLeft -= Time.deltaTime;
        timeLeftInGame -= Time.deltaTime;

        string minutes = Mathf.Floor(timeLeft / 60).ToString("00");
        string seconds = Mathf.Floor(timeLeft % 60).ToString("00");

        if(timeLeft > 0)
        GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>().text = minutes + ":" + seconds;

        if (timeLeftInGame < 0) {
            Application.Quit();
        }
        

        //End round if time is gone

        int playersWithAmmo = 0;
        int notStunnedPlayers = 0;

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

        if (timeLeft < 0 
           || notStunnedPlayers <= 1  || notStunnedPlayers != 0 && playersWithAmmo == 0
             )
        {
			GameManagerShooters.escaped = true;
		}

        foreach (GameObject player in players)
        {
            if (player.GetComponent<ShootAbility>().shootPoints >= 50)
            {
                string temp = player.name.Remove(11);
                winner = temp.Replace("Mod_", " ");
            }
        }

        if (winner != "")
        {
            m_MessageText.text = winner + " WIN!";
            StartCoroutine(WaitBeforeClosingGame());
            
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
            while (!Input.anyKey)
            {
                yield return null;
            }

        Time.timeScale = 1;

        // Increment the round number and display text showing the players what round it is.
        roundNumber++;
       

        //Hide Image
        m_MessageText.text = string.Empty;
		m_MessageImage.CrossFadeAlpha (0, 0.1f, false);

		//Get array of players in scene
		players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.GetComponent<ShootAbility>().enabled = true;
            player.GetComponent<HealAbility>().enabled = true;
        }

		//Set all player's start buttons active
		for (int i = 0; i < players.Length; i++) {
            UIManager.askButton.SetActive(true);
            UIManager.kickButton.SetActive(true);
            UIManager.healButton.SetActive(true);
        }

        //Logging which round started
        //Debug.Log("Round " + m_RoundNumber + " has started" + " sessionID = " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID);
        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID + ", ,"  + "BeginRound" + roundNumber);
		// Wait for the specified length of time until yielding control back to the game loop.
		yield return m_StartWait;

	}

    
	private IEnumerator RoundPlaying ()
	{
		//Set a blocking text to empty during round
		//m_MessageText.text = string.Empty;
	//	m_MessageImage.CrossFadeAlpha (0, 0.1f, false);
	//	CmdHideImage();
		m_MessageText.text = string.Empty;
		m_MessageImage.CrossFadeAlpha (0, 0.1f, false);

       //Playing until 
        while (!escaped)
		{

			// ... return on the next frame.
			yield return null;
		}
        

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

    

    private IEnumerator RoundEnding (){
        

        //wait time before block vision
        yield return beforeEndWait;
        //	StartCoroutine(WaitBeforeBlockingVision());
        spawnPoints = Shift(spawnPoints);

        
   
        for (int i = 0; i < players.Length; i++) {
           
            
            if (players[i].GetComponent<ShootAbility>().alive)
            {
                players[i].GetComponent<ShootAbility>().shootPoints += 1;
            }
            


            
            //reinitialize values 
            UIManager.takeOffTrigger = false;
			UIManager.saveOther = false;
			UIManager.escapeTrigger = false;
			UIManager.trigger = false;
            UIManager.hasFixCharge = true;
            escaped = false;

			//stop player
			players [i].GetComponent<UnityEngine.AI.NavMeshAgent> ().ResetPath();

			//deactivate all buttons 
			UIManager.askButton.SetActive (false);
            UIManager.kickButton.SetActive (false);
            UIManager.shootButton.SetActive(false);
            UIManager.activateButton.SetActive (false);
            UIManager.takeOffButton.SetActive (false);
            UIManager.saveButton.SetActive (false);
            UIManager.fixButton.SetActive (false);
            UIManager.activateGeneratorButton.SetActive(false);
            UIManager.healButton.SetActive(false);
            UIManager.hybernateButton.SetActive(false);


            ParticleSystem.MainModule settings = players[i].GetComponent<ShootAbility>().indicator.GetComponent<ParticleSystem>().main;
            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 0, .5f));

            //Log player's spawn information 
          
           
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID + ", ," + "spawnedTo" + "," + players[i] + "," + spawnPoints[i].transform.position);
           
            //Transform player to a chosen spawnPoint
            // players [i].GetComponent<UseTeleport> ().Cmd_TransformPlayer (players[i], spawnPoint, true);
           
            
            players[i].GetComponent<UseTeleport>().RespawnPlayers(players[i], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation, true);
           
           


            //Shoot logic reset
            players[i].GetComponent<ShootAbility>().hasAmmo = true;
            players[i].GetComponent<ShootAbility>().locked = false;
            players[i].GetComponent<ShootAbility>().stunned = false;
            players[i].GetComponent<ShootAbility>().alive = true;
            players[i].GetComponent<HealAbility>().healOnceBool = false;
            players[i].gameObject.GetComponentInChildren<Animator>().SetBool("Died", false);
            players[i].GetComponent<ShootAbility>().timeForHeal = 7.0f;

        }

      

      

        //for each player in a scene
        for (int i = 0; i < players.Length; i++) {
			
			//Reinitialize speed
			players [i].GetComponent<UnityEngine.AI.NavMeshAgent> ().speed = 1.5f;

		}

		//Reinitialize GameManager's values
		timeLeft = 300.0f;

        //Log about round end
        //Debug.Log ("Round " + m_RoundNumber + " has ended" + " sessionID = " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID);
        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID + ", ," + "EndRound" + roundNumber);

        m_MessageText.text = "ROUND END!";
		m_MessageImage.CrossFadeAlpha (100, 0.1f, false);

		// Wait for the specified length of time until yielding control back to the game loop.
		yield return m_EndWait;

	}

	//Used by GameManager's volume to count saved players
	void OnTriggerEnter(Collider other){
		
		//Adding player to a triggerList
		if(other.CompareTag("Player") && !m_TriggerList.Contains(other)){
			m_TriggerList.Add (other);
            //Reload ammo
            other.GetComponent<ShootAbility>().CmdRechargeAmmo();

		}

        if(other.gameObject == PlayerInfo.localPlayerGameObject)
        {

            UIManager.askButton.SetActive(true);

            UIManager.kickButton.SetActive(false);

            UIManager.healButton.SetActive(false);

            UIManager.wakeUpButton.SetActive(false);
            

        }

		//Block buttons if there are 2 saved players
		if (m_TriggerList.ToArray ().Length > 2) {
			for(int i = 0; i < GameObject.FindGameObjectWithTag("Canvas").transform.childCount; ++i){
			//	GameObject.FindGameObjectWithTag ("Canvas").transform.GetChild (i).gameObject.SetActive(false);
			}

		}

	}

	//Remove player from triggerList when not in a volume
	void OnTriggerExit(Collider other){

		if(other.CompareTag("Player") && m_TriggerList.Contains(other)){
			m_TriggerList.Remove (other);
		}

        if (other.gameObject == PlayerInfo.localPlayerGameObject)
        {
            
               UIManager.fixButton.SetActive(false);

            UIManager.activateGeneratorButton.SetActive(false);



            UIManager.hybernateButton.SetActive(false);

                 UIManager.saveButton.SetActive(false);
           
                UIManager.kickButton.SetActive(true);
            
            if (!other.GetComponent<HealAbility>().healOnceBool)
            {
                UIManager.healButton.SetActive(true);
            }
               
           

            
                UIManager.wakeUpButton.SetActive(true);
            

        }

    }
    */
}
