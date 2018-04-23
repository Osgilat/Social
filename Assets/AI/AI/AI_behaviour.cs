using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class AI_behaviour : MonoBehaviour {

    //Class for managing actor's state and appraisals update 
    private class Actor : MonoBehaviour
    {
        //Better use gameObject Unity property
        public GameObject actorGameObject { get; private set; }

        //Used to track actor's state by his position and health
        public string positionState { get; private set; }
        public string healthState { get; private set; }

        /*
        Used to calculate appraisals to another actors, where GameObject is actor 
        and Vector2 used for appraisal vector representation
        */
        public Dictionary<GameObject, Vector2> DictionaryAppraisalActor { get; set; }

        //Constructor for initializing particular _gameObjectAvatar
        public Actor(GameObject _actorGameObject)
        {

            actorGameObject = _actorGameObject;

            /*Initial position is on 
             * the platform and in alive state */
            positionState = "Platform";
            healthState = "Live";

            //Initialize this actor's appraisals data storage
            DictionaryAppraisalActor = new Dictionary<GameObject, Vector2>();
        }

        //Bad use to apply code to damagedBot behavior
        public Actor(GameObject _actorGameObject, string _damagedBot)
        {

            actorGameObject = _actorGameObject;

            positionState = "Platform";

            healthState = "Live";

        }


        public void CreateAppraisal()
        {
            Actor[] actorsInScene = actorGameObject.GetComponent<AI_behaviour>().actors;
            //Foreach actor in scene
            for (int i = 0; i < actorsInScene.Length; i++)
            {
                //Compare it with every other, including itself
                for (int j = 0; j < actorsInScene.Length; j++)
                {
                    //if self
                    if (j == i)
                    {
                        //Add self to a dictionary with a new blank Vector2
                        actorsInScene[i].DictionaryAppraisalActor.Add(
                            actorsInScene[j].actorGameObject, new Vector2(1.0f, 1.0f));
                    }
                    else
                    {
                        //if not self the add to this dictionary appraisal to another actor with some magic Vector2
                        actorsInScene[i].DictionaryAppraisalActor.Add(
                            actorsInScene[j].actorGameObject, new Vector2(0.1f, 0.1f));
                    }
                }
            }
        }


        public void UpdateStateForActor(Actor actor)
        {

            /*Bad code using trigger to decide positional state, 
             * don't know why not working
             */
            /*

            foreach (Collider avatarGameObject in GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManagerTeleports> ().m_TriggerList)
            {
                if (avatarGameObject.gameObject.Equals (actor.GameObjectAvatar))
                {
                    actor.StatePositionAvatar = "Tower";
                    Debug.Log("On Tower" + avatarGameObject.gameObject.name);
                    break;
                }
                else
                {
                    Debug.Log("On Platform" + avatarGameObject.gameObject.name);
                    actor.StatePositionAvatar = "Platform";

                }
            }

             */


            /*Code use magic transform numbers to decide positional 
             state, that is bad
             */
            if (actor.actorGameObject.transform.position.y > 1.2)
            {
                actor.positionState = "Tower";

            }
            else
            {
                actor.positionState = "Platform";

            }

            //If actor is alive
            if (actor.actorGameObject.GetComponent<ShootAbility>().alive)
            {
                //And in hybernate mode
                if (actor.actorGameObject.GetComponent<HybernationSystem>().isHybernated())
                {
                    //Set his state 
                    actor.healthState = "Hybernate";
                }
                else
                {
                    //If not hybernated but stunned
                    if (actor.actorGameObject.GetComponent<ShootAbility>().stunned)
                    {
                        //Set state to stunned
                        actor.healthState = "Stun";
                    }
                    else
                    {
                        //or normal state
                        actor.healthState = "Normal";
                    }
                }
            }
            else
            {
                actor.healthState = "Dead";
            }

        }
        

        /*Method to update actor's appraisals 
         to another actors*/
        public void UpdateAppraisalActor(Vector2 appraisalAction, GameObject actor, GameObject target)
        {
            Actor[] actorsInScene = actorGameObject.GetComponent<AI_behaviour>().actors;

            //Magic number
            float r = 0.05f;

            /*First appraisal vector
             * for actor who take action
             * second for the target of this action 
              */
            Vector2 appraisalActor = new Vector2();
            Vector2 appraisalTarget = new Vector2();

            //Foreach actor in scene
            for (int i = 0; i < actorsInScene.Length; i++)
            {
                //for actor
                if (actorsInScene[i].actorGameObject.Equals(actor))
                {
                    //try to get old appraisal for target actor and write it to this actor's appraisal
                    actorsInScene[i].DictionaryAppraisalActor.TryGetValue(target, out appraisalActor);

                    //some computation to redefine appraisal of actor based on action's appraisal value
                    appraisalActor = (1 - r) * appraisalActor + (r * AI_behaviour.ComplexConjugate(appraisalAction));

                    //Redefine field in appraisals data storage of this actor to target actor
                    actorsInScene[i].DictionaryAppraisalActor.Remove(target);
                    actorsInScene[i].DictionaryAppraisalActor.Add(target, appraisalActor);
                }

                //for target 
                if (actorsInScene[i].actorGameObject.Equals(target))
                {

                    actorsInScene[i].DictionaryAppraisalActor.TryGetValue(actor, out appraisalTarget);

                    appraisalTarget = (1 - r) * appraisalTarget + (r * appraisalAction);

                    actorsInScene[i].DictionaryAppraisalActor.Remove(actor);
                    actorsInScene[i].DictionaryAppraisalActor.Add(actor, appraisalTarget);
                }
            }
        }



    }

    /*Class used for tracking
     actors positions near of FixZone*/
    private class FixZone : MonoBehaviour
    {

        public GameObject gameObjectFixZone { get; private set; }
        public bool stateFixZone { get; private set; }
        public GameObject whoTaken { get; set; }


        public void UpdateStateFixZone(GameObject _actorGameObject, FixZone fixZone)
        {
            Actor[] actorsInScene = _actorGameObject.GetComponent<AI_behaviour>().actors;

            /*Used to calculate area 
             * near to interactive point
             */
            float xPositionTeleport = fixZone.gameObjectFixZone.transform.position.x;
            float zPositionTeleport = fixZone.gameObjectFixZone.transform.position.z;

            float xPositionAvatar;
            float zPositionAvatar;

            float error = 1f;

            GameObject _whoTaken = null;

            //foreach actor 
            for (int i = 0; i < actorsInScene.Length; i++)
            {
                //Get actor's position
                xPositionAvatar = actorsInScene[i].actorGameObject.transform.position.x;
                zPositionAvatar = actorsInScene[i].actorGameObject.transform.position.z;
                
                //if close to a interactive point
                if (((xPositionTeleport - error) < xPositionAvatar && xPositionAvatar < (xPositionTeleport + error)) &&
                    ((zPositionTeleport - error) < zPositionAvatar && zPositionAvatar < (zPositionTeleport + error)))
                {
                    //store this actor as active in interactive point and break cycle
                    _whoTaken = actorsInScene[i].actorGameObject;
                    break;
                }
                else
                {
                    //actor is not close to interactive point 
                    _whoTaken = null;
                }

            }

            //store variable
            fixZone.whoTaken = _whoTaken;

        }
    }

    /*Class used for tracking
     actors positions near of teleports 
     and teleport's state
     */
    private class Teleport : MonoBehaviour
    {

        public GameObject GameObjectTeleport { get; private set; }
        public bool isActiveTeleport { get; private set; }
        public GameObject activatedBy { get; set; }
        public GameObject occupiedBy { get; set; }

        public Teleport(GameObject _gameObjectTeleport)
        {
            GameObjectTeleport = _gameObjectTeleport;
            isActiveTeleport = false;
            activatedBy = null;
            occupiedBy = null;
        }


        public void UpdateTeleportState(GameObject _actorGameObject, Teleport teleport)
        {
            Actor[] actorsInScene = _actorGameObject.GetComponent<AI_behaviour>().actors;

            teleport.isActiveTeleport = teleport.GameObjectTeleport.GetComponent<ParticleSystem>().isPlaying;

            if (!teleport.isActiveTeleport)
            {
                teleport.activatedBy = null;
            }

            /*Used to calculate area 
             * near to interactive point
             */

            float xPositionTeleport = teleport.GameObjectTeleport.transform.position.x;
            float zPositionTeleport = teleport.GameObjectTeleport.transform.position.z;

            float xPositionAvatar;
            float zPositionAvatar;

            float error = 1f;

            GameObject _occupiedBy = null;

            for (int i = 0; i < actorsInScene.Length; i++)
            {

                xPositionAvatar = actorsInScene[i].actorGameObject.transform.position.x;
                zPositionAvatar = actorsInScene[i].actorGameObject.transform.position.z;

                if (((xPositionTeleport - error) < xPositionAvatar && xPositionAvatar < (xPositionTeleport + error)) &&
                    ((zPositionTeleport - error) < zPositionAvatar && zPositionAvatar < (zPositionTeleport + error)))
                {
                    //store this actor as active in interactive point and break cycle
                    _occupiedBy = actorsInScene[i].actorGameObject;
                    break;
                }
                else
                {
                    _occupiedBy = null;
                }

            }

            //store variable
            teleport.occupiedBy = _occupiedBy;

        }
    }

    /*Class used for action's data structure */
    private class Actions : MonoBehaviour
    {

        public string actionName { get; set; }

        public Vector2 actionAppraisal { get; set; }

        public int actionCharge { get; set; }

        public string positionForActionOfActor { get; set; }

        public string positionForActionOfTarget { get; set; }

        public string stateForActionOfActor { get; set; }

        public string stateForActionOfTarget { get; set; }

        public GameObject targetOfAction { get; set; }

        public GameObject targetForCalculation { get; set; }


        public Actions(string _nameAction, int _charge, Vector2 _appraisalAction, string _statePositionPossibleExecutionActor, string _statePositionPossibleExecutionTarget, string _statePossibleExecutionActor, string _statePossibleExecutionTarget)
        {

            actionName = _nameAction;

            actionAppraisal = _appraisalAction;

            actionCharge = _charge;

            positionForActionOfActor = _statePositionPossibleExecutionActor;

            positionForActionOfTarget = _statePositionPossibleExecutionTarget;

            stateForActionOfActor = _statePossibleExecutionActor;

            stateForActionOfTarget = _statePossibleExecutionTarget;
        }

        public Actions(string _nameAction, Vector2 _appraisalAction, GameObject _target)
        {

            actionName = _nameAction;

            actionAppraisal = _appraisalAction;

            targetOfAction = _target;

            targetForCalculation = _target;
        }

        public Actions(string _nameAction, Vector2 _appraisalAction, GameObject _targetOfAction, GameObject _targetForCalculation)
        {

            actionName = _nameAction;

            actionAppraisal = _appraisalAction;

            targetOfAction = _targetOfAction;

            targetForCalculation = _targetForCalculation;
        }

    }

    /*
    private NavMeshAgent agent;
    
	private float timer;
	private float sessionTimer;

	private static bool key = false;
	private static bool initialized = false;

	private string actionName;
	private GameObject target;

	private static Actor[] actors{ get; set; }
    private static Actor DamagedBot{ get; set; }

    private static Teleport[] teleports{ get; set; }

	private static Actions[] allActions{ get; set; }
	private static Actions[] allStaticActions{ get; set; }

	private static Actions[] ActionDamageBot{ get; set; }

	private static List<Actions> listPossibleAction{ get; set; }
    */

    private NavMeshAgent agent;

    private float timer;
    private float sessionTimer;

    private bool key = false;
    private bool initialized = false;

    private string actionName;
    private GameObject target;

    private Actor[] actors { get; set; }
    private Actor DamagedBot { get; set; }

    private Teleport[] teleports { get; set; }

    private Actions[] allActions { get; set; }
    private Actions[] allStaticActions { get; set; }

    private Actions[] ActionDamageBot { get; set; }

    private List<Actions> listPossibleAction { get; set; }

    private bool moralFlag = false;

	//When scene loaded
	void Start ()
    {
        //After 2s invoke initialization method
		Invoke ("Initialization", 2f);
	}

    private void InitializeTeleports()
    {
        //if teleports exist in these scene
        teleports = new Teleport[2];

        //Store teleports
        teleports[0] = new Teleport(GameObject.FindGameObjectWithTag("Platform_1"));
        teleports[1] = new Teleport(GameObject.FindGameObjectWithTag("Platform_2"));

    }

    private void InitializeActors(int playerCount)
    {
        /*
        Different number of actors
        based on a scene, played
        */
        //	actors = new Actor[4];
        actors = new Actor[playerCount];

    }

    private bool teleportScene = false;
    private bool shootersScene = false;
    private bool passengersScene = false;

    //Initialize any necessary objects
    private void Initialization()
    {

        switch (SceneManager.GetActiveScene().name)
        {
            case "Teleports":
                InitializeTeleports();
                InitializeActors(3);
                teleportScene = true;
                break;
            case "ThreeShooters":
                InitializeActors(3);
                shootersScene = true;
                break;
            case "Passengers":
                InitializeTeleports();
                InitializeActors(4);
                passengersScene = true;
                break;
            default:
                
                break;
        }

       
       
        //Store this AI with special ID
        if (useMoralScheme)
        {
            gameObject.GetComponent<PlayerInfo>().playerID =
            gameObject.GetComponent<PlayerInfo>().playerID + " BOT_moral";
        }
        else
        {
            gameObject.GetComponent<PlayerInfo>().playerID =
            gameObject.GetComponent<PlayerInfo>().playerID + " BOT";
        }
             

        

        //All players gameobjects in scene
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //Used when working with damagedBot
        //GameObject[] playersBot = GameObject.FindGameObjectsWithTag ("DamagedBot");

        //DamagedBot = new Actor (playersBot [0]);

        //From GameObject[] -> actors[], where actors[0] is itself
        //Foreach player in scene 
        for (int i = 0, j = 1; i < players.Length; i++)
        {
            //if damagedBot in scene
            if (players[i].name.Equals("PlayerMod_Damaged(Clone)"))
            {
                DamagedBot = new Actor(players[i]);
            }
            else
            {
                //if itself
                if (players[i] == this.gameObject)
                {
                    //store self in [0] position
                    actors[0] = new Actor(players[i]);
                }
                else
                {
                    //store in (1..) indexes
                    actors[j] = new Actor(players[i]);
                    j++;
                }
            }

        }

        //used for movement of self
        agent = actors[0].actorGameObject.GetComponent<NavMeshAgent>();


        actors[0].CreateAppraisal();

        CreateArrayOfActions();

        //Initialization completed
        initialized = true;
    }

    private float timeForDecision = 5.0f;
    public bool useMoralScheme = false;

    // Update is called once per frame
    private void Update ()
    {

        //Timer for actions iteration
		timer += Time.deltaTime;

        //Keep track of time of session
        sessionTimer += Time.deltaTime;

		if (initialized)
        { 
			
            //Track scene state 
			UpdateAllState ();

            
            if (useMoralScheme)
            {
                MoralScheme();
            }
            

            //Used when work in paradigms with healthState
            /*
			if (timer >= 5.0f && actors[0].LiveState.Equals("Normal")) {

				Selector ();

				key = true;
				timer = 0f;
			}
            */


            // if (timer >= 5.0f && actors[0].positionState.Equals("Platform"))
            if (timer >= timeForDecision && actors[0].positionState.Equals("Platform"))
            {

                timeForDecision = Random.Range(4.0f, 8.0f);

                MakeListOfPossibleAction();

                Selector();

                key = true;

                timer = 0f;
            }

            if (timer >= timeForDecision && actors[0].positionState.Equals("Tower"))
            {
                timeForDecision = Random.Range(4.0f, 8.0f);

                MakeListOfPossibleAction();

                Selector();

                key = true;

                timer = 0f;
            }


            if (key && !blockSave) // flag for action execution
            {
                //Bad code when saving somebody and without charge 
                //   if (!(allActions[3].actionCharge == 0 && actionName == "Saved"))

                //Invoke method based on actionName string 
                //if (!(allActions[3].actionCharge == 0 && actionName == "Saved"))
                //if (!(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerTeleports>().m_TriggerList.Count > 2 && actionName == "Saved"))
                
                if (actionName == "Saved")
                {
                    foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if(player.GetComponent<AI_behaviour>().enabled)
                        {
                            player.GetComponent<AI_behaviour>().blockSave = true;
                        }
                    }
                }

                

                if (actionName != null)
                {
                    Invoke(actionName, 0.01f);
                }
                    
			}
		}
	}

    private bool blockTakeOff = false;

    public bool blockSave = false;

	private void UpdateAllState ()
    {
        
		for (int i = 0; i < actors.Length; i++)
        {
			actors[0].UpdateStateForActor (actors[i]);
		}

        if(!shootersScene)
		for (int i = 0; i < teleports.Length; i++)
        {
			teleports[0].UpdateTeleportState (actors[0].actorGameObject, teleports[i]);
		}


		//Actor.UpdateStateAvatar (DamagedBot);
        
	}

    //Used to define next point for wandering behavior
	private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
		Vector3 randDirection = Random.insideUnitSphere * dist;

		randDirection += origin;

		NavMeshHit navHit;
		NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);

		return navHit.position;
	}

    //Go to random point each wanderTimer seconds
	private void Wander (){

		float wanderRadius = 3.0f;
		float wanderTimer = 3.0f;

		if (timer >= wanderTimer) {
			Vector3 newPos = RandomNavSphere (transform.position, wanderRadius, -1);
			agent.SetDestination (newPos);
			timer = 0;
		}
	}


	public static Vector2 ComplexConjugate(Vector2 vector){
		return Vector2.Scale (vector, new Vector2 (1f, -1f));
	}

    public static Vector2 ComplexMultiplication(Vector2 a, Vector2 b){
		return new Vector2 (a.x * b.x - a.y * b.y , a.x * b.y + a.y * b.x );
	}

    public static float LikelyHood(Vector2 action, Vector2 actor, Vector2 target){
		if ((ComplexMultiplication (action, (ComplexConjugate (actor) + target))).x > 0) {
			return (ComplexMultiplication (action, (ComplexConjugate (actor) + target))).x;
		} else {
			return 0;
		}

	}

    public static float LikelyHoodForMe(Vector2 action, Vector2 actor, Vector2 target){
		if (action.x > 0) {
			return 2 * action.x; //---was: return Mathf.Abs (2 * (action.x * actor.x));
		} else {
			return 0;
		}
	}

    //Used to listen for interactions between actors
	public void ActionListener(string action, GameObject actor, GameObject target)
    {
        if (action == "Saved")
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        for (int i = 0; i < allActions.Length; i++)
        {
            //if action in AI's action list 
            if (allActions[i].actionName.Equals(action))
            {
                //Update appraisal of actor 
                if(!shootersScene)
                actors[0].UpdateAppraisalActor(allActions[i].actionAppraisal, actor, target);

                if(allActions[i].actionName.Equals("Saved"))
                    //&& target.Equals(gameObject))
                {
                  
                    allActions[3].actionCharge = 0;

                }


            }
        }

        if(!shootersScene)
        for(int i = 0; i < allStaticActions.Length; i++)
        {

            if (action.Equals("Activated") || action.Equals("Deactivated") || action.Equals("Escaped"))
            {

                foreach (Actor item in actors)
                {
                    actors[0].UpdateAppraisalActor(allActions[i].actionAppraisal, actor, item.actorGameObject);
                }


                if (action.Equals("Activated"))
                { //---teleport activated!

                    //If not self
                    if (!actor.Equals(actors[0].actorGameObject))
                    {
                        //Change timer for some reason
                        timer = 4.5f; //---change action selection
                                      //Debug.Log ("---TIMER = 4.5F");
                    }

                    
                    for (int j = 0; j < teleports.Length; j++)
                    {
                        //When working with teleports target is not an actor
                        if (teleports[j].GameObjectTeleport.Equals(target))
                        {
                            teleports[j].activatedBy = actor;
                        }
                    }
                }
            }
        }
	}

    //Used for listening for RoundStart and RoundEnd events
	public void ListenerSession(string message){

        //Recreate actions array each round
		if (message.Equals("RoundStart")) {
			CreateArrayOfActions ();

            target = null;
            actionName = null;

            key = false;
		}

		if (message.Equals("RoundEnd")) {
            //Nothing
            timeForDecision = Random.Range(8.0f, 12.0f);
            target = null;
            actionName = null;

            blockTakeOff = false;
            blockSave = false;
            key = false;
		}

	}

    //Creates array of actions with all necessary information about action
	private void CreateArrayOfActions(){

        if (teleportScene)
        {
            allActions = new Actions[4];

            allActions[0] = new Actions("Ask", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[1] = new Actions("Kick", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[2] = new Actions("Thank", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[3] = new Actions("Saved", 1, new Vector2(0.10f, 0.15f), "Tower", "Platform", "Normal", "Normal");

            allStaticActions = new Actions[2];

            allStaticActions[0] = new Actions("TakeOff", 1, new Vector2(2.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allStaticActions[1] = new Actions("Activated", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");

        } 
        else if (shootersScene)
        {
            allActions = new Actions[7];

            allActions[0] = new Actions("Ask", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[1] = new Actions("Kick", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[2] = new Actions("Thank", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[3] = new Actions("Target", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[4] = new Actions("Shoot", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[5] = new Actions("Heal", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Stun");
            allActions[6] = new Actions("Missed", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
        } 
        else if (passengersScene)
        {
            allActions = new Actions[9];

            allActions[0] = new Actions("Ask", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[1] = new Actions("Kick", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[2] = new Actions("Thank", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[3] = new Actions("Target", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[4] = new Actions("Shoot", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[5] = new Actions("Heal", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Stun");
            allActions[6] = new Actions("Missed", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allActions[7] = new Actions("Hybernate", 1, new Vector2(0.10f, 0.15f), "Tower", "Tower", "Normal", "Normal");
            allActions[8] = new Actions("WakeUp", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Hybernate");

            allStaticActions = new Actions[3];

            allStaticActions[0] = new Actions("TakeOff", 1, new Vector2(2.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allStaticActions[1] = new Actions("Activated", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
            allStaticActions[2] = new Actions("Fix", 1, new Vector2(0.10f, 0.15f), "Tower", "Tower", "Normal", "Normal");

        }


        /*
		
		Action[9] = new Actions("Saved", 1, new Vector2(0.10f, 0.15f), "Tower", "Platform", "Normal", "Normal");
        */

        /*
		ActionStatic = new Actions[3];

		ActionStatic[0] = new Actions("Fix", 1, new Vector2(0.10f, 0.15f), "Tower", "Tower", "Normal", "Normal");
		ActionStatic[1] = new Actions("TakeOff", 1, new Vector2(2.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
		ActionStatic[2] = new Actions("Activated", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
        */
        /*
		ActionDamageBot = new Actions[1];

		ActionDamageBot[0] = new Actions("Kick", 1, new Vector2(3.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
        */
        //Action[1] = new Actions("Shoot", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");

    }

	//Create actions based on states of an actors
	private void MakeListOfPossibleAction(){

		listPossibleAction = new List<Actions>();

		for (int i = 0; i < allActions.Length; i++)
        {
			
            //if action have not happened in this round
			if (allActions[i].actionCharge == 1)
			{
                //foreach actor but not for itself
				for (int j = 1; j < actors.Length; j++)
				{
                    /*if target actor in position for action 
                     && itself in position to act 
                     && target actor in proper health state
                     && itself in proper health state
                     */
					if (actors[j].positionState.Equals(allActions[i].positionForActionOfTarget)
                        && actors[0].positionState.Equals(allActions[i].positionForActionOfActor)
                        && actors[j].healthState.Equals(allActions[i].stateForActionOfTarget) 
                        && actors[0].healthState.Equals(allActions[i].stateForActionOfActor))
					{
                        //Add such action to a list of possible
						listPossibleAction.Add(new Actions(allActions[i].actionName, allActions[i].actionAppraisal, actors[j].actorGameObject));
					}
				}
			}
		}

        //for each static action
        if(!shootersScene)
		for (int i = 0; i < allStaticActions.Length; i++) {

            //if action have not happened in this round
            if (allStaticActions [i].actionCharge == 1)
            {
				/*
                 if itself position == position to target actor
                 && itself position proper to act
                 && itself health state == target state
                 && itself health state == state to act
                 */
				if (actors [0].positionState.Equals (allStaticActions [i].positionForActionOfTarget)
                    && actors [0].positionState.Equals (allStaticActions [i].positionForActionOfActor)
                    && actors [0].healthState.Equals (allStaticActions [i].stateForActionOfTarget)
                    && actors [0].healthState.Equals (allStaticActions [i].stateForActionOfActor))
                {

					if (allStaticActions [i].actionName.Equals ("Fix"))
                    {
						listPossibleAction.Add (new Actions (allStaticActions [i].actionName, allStaticActions [i].actionAppraisal, actors [0].actorGameObject));
					}

					if (allStaticActions [i].actionName.Equals ("TakeOff"))
                    {
						for (int j = 0; j < teleports.Length; j++)
                        {
                            /*
                             if teleport is active 
                             && activated not by itself
                             */
							if (teleports [j].isActiveTeleport 
                                && !teleports [j]
                                .activatedBy
                                .Equals (actors [0].actorGameObject))
                            {
								listPossibleAction.Add (new Actions (allStaticActions [i].actionName,
                                    allStaticActions [i].actionAppraisal,
                                    teleports [j].GameObjectTeleport,
                                    actors [0].actorGameObject));
							}
						}
					}
				}

				if (allStaticActions [i].actionName.Equals ("Activated"))
                {
                    
					for (int j = 1; j < actors.Length; j++)
                    {
						for (int k = 0; k < teleports.Length; k++)
                        {
                            /*
                             if actor and target selected properly 
                             && (teleport is free || teleport occupied by itself)
                             */
							if ((actors [j].positionState.Equals (allStaticActions [i].positionForActionOfTarget) 
                                && actors [0].positionState.Equals (allStaticActions [i].positionForActionOfActor) 
                                && actors [j].healthState.Equals (allStaticActions [i].stateForActionOfTarget) 
                                && actors [0].healthState.Equals (allStaticActions [i].stateForActionOfActor)) 
                                && (teleports [k].occupiedBy == null 
                                || teleports [k].occupiedBy.Equals (actors [0].actorGameObject)))
                            {
								listPossibleAction.Add (new Actions (allStaticActions [i].actionName, allStaticActions [i].actionAppraisal, teleports [k].GameObjectTeleport, actors [j].actorGameObject));
							}
						}
					}
				}
			}
		}

        /*
		for (int i = 0; i < ActionDamageBot.Length; i++) {

			if (ActionDamageBot[i].Charge == 1)
			{
				if (DamagedBot.StatePositionAvatar.Equals(ActionDamageBot[i].StatePositionPossibleExecutionTarget) && actors[0].StatePositionAvatar.Equals(ActionDamageBot[i].StatePositionPossibleExecutionActor) && 
					DamagedBot.LiveState.Equals(ActionDamageBot[i].StatePossibleExecutionTarget) && actors[0].LiveState.Equals(ActionDamageBot[i].StatePossibleExecutionActor))
				{
					ListPossibleAction.Add(new Actions(ActionDamageBot[i].actionName, ActionDamageBot[i].actionAppraisal, DamagedBot.GameObjectAvatar));
				}
			}

		}
        */

	}


	private void Selector()
    {

		//???MakeListOfPossibleAction ();

		Vector2 appraisalActor0 = new Vector2();

		Vector2 appraisalActor1 = new Vector2();
		Vector2 appraisalActor2 = new Vector2();
		//Vector2 appraisalActor3 = new Vector2();

		Vector2 appraisalTarget1 = new Vector2();
		Vector2 appraisalTarget2 = new Vector2();
	//	Vector2 appraisalTarget3 = new Vector2();

		actors[0].DictionaryAppraisalActor.TryGetValue (actors [0].actorGameObject, out appraisalActor0);

		actors[0].DictionaryAppraisalActor.TryGetValue (actors [1].actorGameObject, out appraisalActor1);

        actors[0].DictionaryAppraisalActor.TryGetValue(actors[2].actorGameObject, out appraisalActor2);

        actors[1].DictionaryAppraisalActor.TryGetValue (actors [0].actorGameObject, out appraisalTarget1);

		actors [2].DictionaryAppraisalActor.TryGetValue (actors [0].actorGameObject, out appraisalTarget2);

        /*
        actors [0].DictionaryAppraisalActor.TryGetValue (actors [3].GameObjectAvatar, out appraisalActor3);
		actors [3].DictionaryAppraisalActor.TryGetValue (actors [0].GameObjectAvatar, out appraisalTarget3);
        */

		float sum = 0.0f;

		float sum1 = 0.0f;

		for (int i = 0; i < listPossibleAction.Count; i++) {

			if (listPossibleAction [i].targetForCalculation.Equals (actors [0].actorGameObject))
            {
				sum += LikelyHoodForMe (listPossibleAction [i].actionAppraisal, appraisalActor0, appraisalActor0);
			}
			if (listPossibleAction [i].targetForCalculation.Equals (actors [1].actorGameObject))
            {
				sum += LikelyHood (listPossibleAction [i].actionAppraisal, appraisalActor1, appraisalTarget1);
			}
			if (listPossibleAction [i].targetForCalculation.Equals (actors [2].actorGameObject))
            {
				sum += LikelyHood (listPossibleAction [i].actionAppraisal, appraisalActor2, appraisalTarget2);
			}
            /*
			if (ListPossibleAction [i].TargetForCalculation.Equals (actors [3].GameObjectAvatar)) {
				sum += LikelyHood (ListPossibleAction [i].actionAppraisal, appraisalActor3, appraisalTarget3);
			}
            */
		}

		float rand = Random.Range (0.0f, sum);

		for (int i = 0; i < listPossibleAction.Count; i++)
        {

			if (listPossibleAction [i].targetForCalculation.Equals (actors [0].actorGameObject))
            {
				sum1 += LikelyHoodForMe (listPossibleAction [i].actionAppraisal, appraisalActor0, appraisalActor0);
			}
			if (listPossibleAction [i].targetForCalculation.Equals (actors [1].actorGameObject))
            {
				sum1 += LikelyHood (listPossibleAction [i].actionAppraisal, appraisalActor1, appraisalTarget1);
			}
			if (listPossibleAction [i].targetForCalculation.Equals (actors [2].actorGameObject))
            {
				sum1 += LikelyHood (listPossibleAction [i].actionAppraisal, appraisalActor2, appraisalTarget2);
			}
            /*
			if (ListPossibleAction [i].TargetForCalculation.Equals (actors [3].GameObjectAvatar)) {
				sum1 += LikelyHood (ListPossibleAction [i].actionAppraisal, appraisalActor3, appraisalTarget3);
			}
            */


			if (rand < sum1)
            {

				target = listPossibleAction [i].targetOfAction;

				actionName = listPossibleAction [i].actionName;

				break;
			}

		}
	}


	private void MoralScheme (){
		if (!moralFlag)
        {
			if (sessionTimer > 180)
            {

				GameObject partner;
				GameObject competitor;

				Vector2 appraisalActor1 = new Vector2 ();
				Vector2 appraisalActor2 = new Vector2 ();
			//	Vector2 appraisalActor3 = new Vector2 ();

				//float[] valenceActor;

				actors [1].DictionaryAppraisalActor.TryGetValue (actors [0].actorGameObject, out appraisalActor1);
				actors [2].DictionaryAppraisalActor.TryGetValue (actors [0].actorGameObject, out appraisalActor2);
			//	actors [3].DictionaryAppraisalActor.TryGetValue (actors [0].GameObjectAvatar, out appraisalActor3);

				//float[] valenceActor = new float[] {appraisalActor1.x, appraisalActor2.x, appraisalActor3.x};

				if (appraisalActor1.x >= appraisalActor2.x) {
					partner = actors [1].actorGameObject;
					competitor = actors [2].actorGameObject;
				} else {
					partner = actors [2].actorGameObject;
					competitor = actors [1].actorGameObject;
				}
					
				actors[0].UpdateAppraisalActor(new Vector2 (20.0f, 0.0f), actors [0].actorGameObject, partner);
				actors[0].UpdateAppraisalActor(new Vector2 (-20.0f, 0.0f), actors [0].actorGameObject, competitor);

				moralFlag = true;

			}
		}
	}
		


	private void TakeOff ()
    {

		agent.SetDestination (target.transform.position);

		float dist = Vector3.Distance (gameObject.transform.position, target.transform.position);

		if (dist < 3.0f)
        {
            if (!blockTakeOff)
            {

                blockTakeOff = true;

            }
            else
            {
                return;
            }

            StartCoroutine(AttemptTakeOff());

			
		}


	}

    private IEnumerator AttemptTakeOff()
    {
        yield return new WaitForSeconds(Random.Range(4.0f, 7.0f));

        gameObject.GetComponent<UseTeleport>().AI_TakeOff();
        key = false;
    }



    private void Ask ()
    {
		
		gameObject.GetComponent<PlayerActor>().AI_Ask(target);

		allActions[0] = new Actions("Ask", 0, new Vector2(0.10f, -0.20f), "Platform", "Platform", "Live", "Normal");

		key = false;
	}



    private void Thank()
    {
		
		gameObject.GetComponent<PlayerActor>().AI_Thank(target);

		allActions[2] = new Actions("Thank", 0, new Vector2(0.10f, -0.20f), "Platform", "Platform", "Live", "Normal");

		key = false;
	}



    private void Kick ()
    {

		agent.SetDestination (target.transform.position);

		float dist = Vector3.Distance (gameObject.transform.position, target.transform.position);

		if (dist < 3.0f) {
            agent.ResetPath();
            gameObject.GetComponent<PushAbility>().AI_AttackPlayer(target);

			key = false;
		}
	}

    private void Target()
    {
		
		gameObject.GetComponent<ShootAbility>().AI_Target(target);

		key = false;
	}

    private void Shoot()
    {
		
		gameObject.GetComponent<ShootAbility>().AI_Target(target);

		Invoke("ShootShoot", 1.0f);

		allActions[3] = new Actions("Target", 0, new Vector2(-0.50f, 0.30f), "Platform", "Platform", "Live", "Normal");
		allActions[4] = new Actions("Shoot", 0, new Vector2(-0.70f, 0.40f), "Platform", "Platform", "Live", "Normal");
		allActions[6] = new Actions("Missed", 0, new Vector2(0.50f, 0.20f), "Platform", "Platform", "Live", "Normal");

		key = false;
	}

    private void Heal()
	{
		gameObject.GetComponent<HealAbility>().AI_Heal(target);

		allActions[5] = new Actions("Heal", 0, new Vector2(0.90f, -0.20f), "Platform", "Platform", "Live", "Stun");

		key = false;
	}

    private void Missed()
	{
		gameObject.GetComponent<ShootAbility>().AI_Shoot();

		allActions[3] = new Actions("Target", 0, new Vector2(-0.50f, 0.30f), "Platform", "Platform", "Live", "Normal");
		allActions[4] = new Actions("Shoot", 0, new Vector2(-0.70f, 0.40f), "Platform", "Platform", "Live", "Normal");
		allActions[6] = new Actions("Missed", 0, new Vector2(0.50f, 0.20f), "Platform", "Platform", "Live", "Normal");

		key = false;
	}

    private void ShootShoot()
	{
		gameObject.GetComponent<ShootAbility>().AI_Shoot();

		key = false;
	}


    private void Activated ()
    {

		agent.SetDestination (target.transform.position);

		float dist = Vector3.Distance (gameObject.transform.position, target.transform.position);

		if (dist < 3.0f) {
            //gameObject.GetComponent<UseTeleport>().AI_ActivateTeleport(gameObject);
            gameObject.GetComponent<UseTeleport>().AI_ActivateTeleport();
            key = false;
		}
			
	}

    private void Saved (){
        if(actors[0].positionState.Equals("Tower"))
        {
            //gameObject.GetComponent<UseTeleport>().AI_ActivateSaveButton(target);

            // gameObject.GetComponent<UseTeleport>().AI_Save(target);

            StartCoroutine(AttemptSave());

            /*
             * 
            //Transform hitted player
            GetComponent<UseTeleport>().Cmd_TransformPlayer(target,
                GetComponent<UseTeleport>().pos_2.transform.position,
                GetComponent<UseTeleport>().pos_2.transform.rotation, false);
                */
            
                //= new Actions("Saved", 0, new Vector2(0.10f, 0.15f), "Tower", "Platform", "Normal", "Normal");

            
        }
     
	}

    private IEnumerator AttemptSave()
    {
        yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));
        gameObject.GetComponent<UseTeleport>().AI_Save(target);
        allActions[3].actionCharge = 0;
        key = false;
    }


    private void WakeUp(){

		agent.SetDestination (target.transform.position);

		float dist = Vector3.Distance (gameObject.transform.position, target.transform.position);

		if (dist < 3.0f) {
            agent.ResetPath();
			gameObject.GetComponent<WakeUpAbility> ().AI_WakeUp (target);

			key = false;
		}
	}

    
    private void Fix (){

        
        gameObject.GetComponent<FixAbility>().AI_Fix();

        allStaticActions[0] = new Actions("Fix", 0, new Vector2(0.10f, 0.15f), "Tower", "Tower", "Normal", "Normal");

		key = false;
	}

    private void Hybernate()
    {
		gameObject.GetComponent<HybernationSystem> ().AI_Hybernate (target);

		key = false;
	}
		
}
