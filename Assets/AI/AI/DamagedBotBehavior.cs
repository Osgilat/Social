using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DamagedBotBehavior : MonoBehaviour {

    private class Avatar : MonoBehaviour
    {

        public GameObject GameObjectAvatar { get; private set; }

        public string StatePositionAvatar { get; private set; }

        public string StateAvatarLive { get; private set; }

        public Dictionary<GameObject, Vector2> DictionaryAppraisalActor { get; set; }

        public Avatar(GameObject _gameObjectAvatar)
        {

            GameObjectAvatar = _gameObjectAvatar;

            StatePositionAvatar = "Platform";

            StateAvatarLive = "Live";

            DictionaryAppraisalActor = new Dictionary<GameObject, Vector2>();
        }

        public Avatar(GameObject _gameObjectAvatar, string _damagedBot)
        {

            GameObjectAvatar = _gameObjectAvatar;

            StatePositionAvatar = "Platform";

            StateAvatarLive = "Live";

        }
        public static void CreateAppraisal()
        {

            for (int i = 0; i < DamagedBotBehavior.Avatars.Length; i++)
            {

                for (int j = 0; j < DamagedBotBehavior.Avatars.Length; j++)
                {
                    if (j == i)
                    {
                        DamagedBotBehavior.Avatars[i].DictionaryAppraisalActor.Add(DamagedBotBehavior.Avatars[j].GameObjectAvatar, new Vector2(1.0f, 1.0f));
                    }
                    else
                    {
                        DamagedBotBehavior.Avatars[i].DictionaryAppraisalActor.Add(
                            DamagedBotBehavior.Avatars[j].GameObjectAvatar, new Vector2(0.1f, 0.1f));
                    }
                }
            }
        }
        /*
        public static void CreateAppraisal (){

            for (int i = 0; i < Behaviour.actors.Length; i++) {

                for (int j = 0; j < Behaviour.actors.Length; j++) {
                    if (j == i) {
                        Behaviour.actors [i].DictionaryAppraisalActor.Add (Behaviour.actors [j].GameObjectAvatar, new Vector2 (1.0f, 1.0f));
                    } else {
                        Behaviour.actors [i].DictionaryAppraisalActor.Add (
                            Behaviour.actors [j].GameObjectAvatar, new Vector2 (0.1f, 0.1f));
                    }
                } 
            }
        }
        */


        public static void UpdateStateAvatar(Avatar avatar)
        {
            /*

            foreach (Collider avatarGameObject in GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManagerTeleports> ().m_TriggerList)
            {
                if (avatarGameObject.gameObject.Equals (avatar.GameObjectAvatar))
                {
                    avatar.StatePositionAvatar = "Tower";
                    Debug.Log("On Tower" + avatarGameObject.gameObject.name);
                    break;
                }
                else
                {
                    Debug.Log("On Platform" + avatarGameObject.gameObject.name);
                    avatar.StatePositionAvatar = "Platform";

                }
            }

             */

            if (avatar.GameObjectAvatar.transform.position.y > 1.2)
            {
                avatar.StatePositionAvatar = "Tower";

            }
            else
            {
                avatar.StatePositionAvatar = "Platform";

            }

            if (avatar.GameObjectAvatar.GetComponent<ShootAbility>().alive)
            {
                if (avatar.GameObjectAvatar.GetComponent<HybernationSystem>().isHybernated())
                {
                    avatar.StateAvatarLive = "Hybernate";
                }
                else
                {
                    if (avatar.GameObjectAvatar.GetComponent<ShootAbility>().stunned)
                    {
                        avatar.StateAvatarLive = "Stun";
                    }
                    else
                    {
                        avatar.StateAvatarLive = "Normal";
                    }
                }
            }
            else
            {
                avatar.StateAvatarLive = "Dead";
            }

        }

        public static void UpdateAppraisalAvatar(Vector2 appraisalAction, GameObject actor, GameObject target)
        {

            float r = 0.05f;

            Vector2 appraisalActor = new Vector2();
            Vector2 appraisalTarget = new Vector2();

            for (int i = 0; i < DamagedBotBehavior.Avatars.Length; i++)
            {
                if (DamagedBotBehavior.Avatars[i].GameObjectAvatar.Equals(actor))
                {

                    DamagedBotBehavior.Avatars[i].DictionaryAppraisalActor.TryGetValue(target, out appraisalActor);

                    appraisalActor = (1 - r) * appraisalActor + (r * AI_behaviour.ComplexConjugate(appraisalAction));

                    DamagedBotBehavior.Avatars[i].DictionaryAppraisalActor.Remove(target);
                    DamagedBotBehavior.Avatars[i].DictionaryAppraisalActor.Add(target, appraisalActor);
                }

                if (DamagedBotBehavior.Avatars[i].GameObjectAvatar.Equals(target))
                {

                    DamagedBotBehavior.Avatars[i].DictionaryAppraisalActor.TryGetValue(actor, out appraisalTarget);

                    appraisalTarget = (1 - r) * appraisalTarget + (r * appraisalAction);

                    DamagedBotBehavior.Avatars[i].DictionaryAppraisalActor.Remove(actor);
                    DamagedBotBehavior.Avatars[i].DictionaryAppraisalActor.Add(actor, appraisalTarget);
                }
            }
        }



    }

    private class FixZone : MonoBehaviour
    {

        public GameObject GameObjectFixZone { get; private set; }
        public bool StateFixZone { get; private set; }
        public GameObject WhomTaken { get; set; }


        public static void UpdateStateFixZone(FixZone fixZone)
        {

            float xPositionTeleport = fixZone.GameObjectFixZone.transform.position.x;
            float zPositionTeleport = fixZone.GameObjectFixZone.transform.position.z;

            float xPositionAvatar;
            float zPositionAvatar;

            float error = 1f;

            GameObject _whoTaken = null;

            for (int i = 0; i < DamagedBotBehavior.Avatars.Length; i++)
            {

                xPositionAvatar = DamagedBotBehavior.Avatars[i].GameObjectAvatar.transform.position.x;
                zPositionAvatar = DamagedBotBehavior.Avatars[i].GameObjectAvatar.transform.position.z;

                if (((xPositionTeleport - error) < xPositionAvatar && xPositionAvatar < (xPositionTeleport + error)) &&
                    ((zPositionTeleport - error) < zPositionAvatar && zPositionAvatar < (zPositionTeleport + error)))
                {

                    _whoTaken = DamagedBotBehavior.Avatars[i].GameObjectAvatar;
                    break;
                }
                else
                {
                    _whoTaken = null;
                }

            }

            fixZone.WhomTaken = _whoTaken;

        }
    }

    private class Actions : MonoBehaviour
    {

        public string NameAction { get; set; }

        public Vector2 AppraisalAction { get; set; }

        public int Charge { get; set; }

        public string StatePositionPossibleExecutionActor { get; set; }

        public string StatePositionPossibleExecutionTarget { get; set; }

        public string StatePossibleExecutionActor { get; set; }

        public string StatePossibleExecutionTarget { get; set; }

        public GameObject TargetForAction { get; set; }

        public GameObject TargetForCalculation { get; set; }


        public Actions(string _nameAction, int _charge, Vector2 _appraisalAction, string _statePositionPossibleExecutionActor, string _statePositionPossibleExecutionTarget, string _statePossibleExecutionActor, string _statePossibleExecutionTarget)
        {

            NameAction = _nameAction;

            AppraisalAction = _appraisalAction;

            Charge = _charge;

            StatePositionPossibleExecutionActor = _statePositionPossibleExecutionActor;

            StatePositionPossibleExecutionTarget = _statePositionPossibleExecutionTarget;

            StatePossibleExecutionActor = _statePossibleExecutionActor;

            StatePossibleExecutionTarget = _statePossibleExecutionTarget;
        }

        public Actions(string _nameAction, Vector2 _appraisalAction, GameObject _target)
        {

            NameAction = _nameAction;

            AppraisalAction = _appraisalAction;

            TargetForAction = _target;

            TargetForCalculation = _target;
        }

        public Actions(string _nameAction, Vector2 _appraisalAction, GameObject _targetForAction, GameObject _targetForCalculation)
        {

            NameAction = _nameAction;

            AppraisalAction = _appraisalAction;

            TargetForAction = _targetForAction;

            TargetForCalculation = _targetForCalculation;
        }

    }

    private class Teleport : MonoBehaviour
    {

        public GameObject GameObjectTeleport { get; private set; }
        public bool StateTeleport { get; private set; }
        public GameObject WhoActivated { get; set; }
        public GameObject WhomTaken { get; set; }

        public Teleport(GameObject _gameObjectTeleport)
        {
            GameObjectTeleport = _gameObjectTeleport;
            StateTeleport = false;
            WhoActivated = null;
            WhomTaken = null;
        }

        public static void UpdateStateTeleport(Teleport teleport)
        {

            teleport.StateTeleport = teleport.GameObjectTeleport.GetComponent<ParticleSystem>().isPlaying;

            if (!teleport.StateTeleport)
            {
                teleport.WhoActivated = null;
            }

            float xPositionTeleport = teleport.GameObjectTeleport.transform.position.x;
            float zPositionTeleport = teleport.GameObjectTeleport.transform.position.z;

            float xPositionAvatar;
            float zPositionAvatar;

            float error = 1f;

            GameObject _whoTaken = null;

            for (int i = 0; i < DamagedBotBehavior.Avatars.Length; i++)
            {

                xPositionAvatar = DamagedBotBehavior.Avatars[i].GameObjectAvatar.transform.position.x;
                zPositionAvatar = DamagedBotBehavior.Avatars[i].GameObjectAvatar.transform.position.z;

                if (((xPositionTeleport - error) < xPositionAvatar && xPositionAvatar < (xPositionTeleport + error)) &&
                    ((zPositionTeleport - error) < zPositionAvatar && zPositionAvatar < (zPositionTeleport + error)))
                {

                    _whoTaken = DamagedBotBehavior.Avatars[i].GameObjectAvatar;
                    break;
                }
                else
                {
                    _whoTaken = null;
                }

            }

            teleport.WhomTaken = _whoTaken;

        }
    }

    private float timer;
	private float timerForWander;

    private NavMeshAgent agent;

	private GameObject target;
	private int actionNumber;

	private static bool key = false;
	private static bool initialized = false;

	private static Avatar[] Avatars { get; set; }
	private static Actions[] ActionDamageBot{ get; set; }
	private static List<Actions> ListPossibleActionDamageBot{ get; set; }



	// Use this for initialization
    void Start ()
    {
		Invoke ("Initialization", 2f);
	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;
		timerForWander += Time.deltaTime;

		if (initialized) {
			UpdateAllState ();

			if (timer >= 10.0f) {

				MakeListOfPossibleAction ();

				Selector ();

				key = true;
				timer = 0f;
			}

			if(key){
                //Invoke (ListPossibleActionDamageBot [actionNumber].NameAction, 0.01f);
                Invoke(ActionDamageBot[actionNumber].NameAction, 0.01f);
                

            }
		}
	}
		
	private void Initialization (){
		agent = gameObject.GetComponent<NavMeshAgent>();

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		GameObject[] playersBot = GameObject.FindGameObjectsWithTag ("DamagedBot");

        Avatars = new Avatar[5];

        Avatars[0] = new Avatar (playersBot [0], "damage");

		for (int i = 0, j = 1; i < players.Length; i++) {
            Avatars[j] = new Avatar (players [i], "damage");
			j++;
		}

		CreateArrayAction ();

		timer = 5.0f;
		initialized = true;
	}

	private void UpdateAllState () {
		for (int i = 0; i < Avatars.Length; i++) {
			Avatar.UpdateStateAvatar (Avatars[i]);
		}
	}

	private void Selector ()
    {
        /*
		actionNumber = Random.Range (0, ListPossibleActionDamageBot.Count - 1);


        target = ListPossibleActionDamageBot [actionNumber]
            .TargetForAction;
            */
        actionNumber = Random.Range(0, ActionDamageBot.Length);

        target =
            Avatars[Random.Range(0, Avatars.Length)].GameObjectAvatar;

        //Debug.Log (ListPossibleActionDamageBot [actionNumber].actionName + " " + target.name);
    }
		
	private void CreateArrayAction()
    {

		ActionDamageBot = new Actions[3];

		ActionDamageBot[0] = new Actions("WakeUp", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Hybernate");
        
		ActionDamageBot[1] = new Actions("Kick", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
		ActionDamageBot[2] = new Actions("Wander", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
        /*
		ActionDamageBot[3] = new Actions("Target", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
		ActionDamageBot[4] = new Actions("Shoot", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
		ActionDamageBot[5] = new Actions("Missed", 1, new Vector2(0.10f, 0.15f), "Platform", "Platform", "Normal", "Normal");
        */
	}
		
	public void ListenerSession(string message){

		if (message.Equals("RoundStart")) {
			CreateArrayAction ();
		}

		if (message.Equals("RoundEnd")) {
			//Do nothing
		}
	}

	private void MakeListOfPossibleAction(){

		ListPossibleActionDamageBot = new List<Actions>();

		for (int i = 0; i < ActionDamageBot.Length; i++)
        {
			if (ActionDamageBot[i].Charge == 1)
            {
				for (int j = 1; j < Avatars.Length; j++)
                {
					if (Avatars[j].StatePositionAvatar.Equals(ActionDamageBot[i].StatePositionPossibleExecutionTarget)
                        && Avatars[0].StatePositionAvatar.Equals(ActionDamageBot[i].StatePositionPossibleExecutionActor)
                        && Avatars[j].StateAvatarLive.Equals(ActionDamageBot[i].StatePossibleExecutionTarget) 
                        && Avatars[0].StateAvatarLive.Equals(ActionDamageBot[i].StatePossibleExecutionActor))
					{
						ListPossibleActionDamageBot.Add(new Actions(ActionDamageBot[i].NameAction, ActionDamageBot[i].AppraisalAction, Avatars[j].GameObjectAvatar));
					}
				}
			}
		}
	}


	public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
		Vector3 randDirection = Random.insideUnitSphere * dist;

		randDirection += origin;

		NavMeshHit navHit;
		NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);

		return navHit.position;
	}


	private void Wander ()
    {

		float wanderRadius = 3.0f;
		float wanderTimer = 3.0f;

		if (timerForWander >= wanderTimer) {
			Vector3 newPos = RandomNavSphere (transform.position, wanderRadius, -1);
			agent.SetDestination (newPos);
			timerForWander = 0f;
		}
	}


	private void WakeUp()
    {

        agent.SetDestination(target.transform.position);

        float dist = Vector3.Distance(gameObject.transform.position, target.transform.position);

        if (dist < 3.0f)
        {
            agent.ResetPath();
			gameObject.GetComponent<WakeUpAbility>().AI_WakeUp(target);
			key = false;
		}

    }

	private void Target(){

		gameObject.GetComponent<ShootAbility>().AI_Target(target);

	}

	private void Shoot(){

		gameObject.GetComponent<ShootAbility>().AI_Target(target);

		Invoke("ShootShoot", 1.0f);

		ActionDamageBot[3] = new Actions("Target", 0, new Vector2(-0.50f, 0.30f), "Platform", "Platform", "Live", "Normal");
		ActionDamageBot[4] = new Actions("Shoot", 0, new Vector2(-0.70f, 0.40f), "Platform", "Platform", "Live", "Normal");
		ActionDamageBot[5] = new Actions("Missed", 0, new Vector2(0.50f, 0.20f), "Platform", "Platform", "Live", "Normal");

	}
		
	private void Missed()
	{
		gameObject.GetComponent<ShootAbility>().AI_Shoot();

		ActionDamageBot[3] = new Actions("Target", 0, new Vector2(-0.50f, 0.30f), "Platform", "Platform", "Live", "Normal");
		ActionDamageBot[4] = new Actions("Shoot", 0, new Vector2(-0.70f, 0.40f), "Platform", "Platform", "Live", "Normal");
		ActionDamageBot[5] = new Actions("Missed", 0, new Vector2(0.50f, 0.20f), "Platform", "Platform", "Live", "Normal");

		key = false;
	}



	private void ShootShoot(){
        gameObject.GetComponent<ShootAbility>().AI_Shoot();

		key = false;
    }

	private void Kick(){

        agent.SetDestination(target.transform.position);

        float dist = Vector3.Distance(gameObject.transform.position, target.transform.position);

        if (dist < 3.0f)
        {
            agent.ResetPath();
            gameObject.GetComponent<PushAbility>().AI_AttackPlayer(target);

			key = false;
        }
    }
    
}
