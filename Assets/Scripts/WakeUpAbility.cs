using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WakeUpAbility : NetworkBehaviour
{
    public float rotationSpeed;
    public ParticleSystem kickEffect;

    //Used to control button's behavior
    public static bool wakeUp = false; 

    private Vector3 targetDir;

    //Reference to a audioSync script
    private AudioSync audioSync;

    //Reference to a button
    private static GameObject wakeUpButton;

    //Reference to animator
    private Animator animator;		

    public void WakeUpOnClick()
    {
        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        PlayerInfo.UIManager.ResetUI(PlayerInfo.UIManager.wakeUpButton);

        if (!wakeUp)
        {
            wakeUp = true;
            GameObject.FindGameObjectWithTag("WakeUpButton").GetComponent<Image>().color = Color.clear;

        }
        else
        {
            wakeUp = false;
            GameObject.FindGameObjectWithTag("WakeUpButton").GetComponent<Image>().color = Color.white;
        }




    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        audioSync = GetComponent<AudioSync>();
        

    }

    // Update is called once per frame
    void Update () {
        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        if (Input.GetMouseButton(0) && wakeUp)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                
                if (hit.transform.CompareTag("Player"))
                {
                    
                    Cmd_CollidersIdentify(this.transform.position, 2.5f, hit.transform.gameObject);

                }
            }
        }
    }

    //Function to AI control
    public void AI_WakeUp(GameObject ai_target)
    {

        Cmd_CollidersIdentify(this.transform.position, 3f, ai_target);

    }

    [Command]
    void Cmd_CollidersIdentify(Vector3 center, float radius, GameObject target)
    {
        //Take all coliders in a radius
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //If player in radius
            if (hitColliders[i].tag == "Player" && hitColliders[i].gameObject != gameObject)
            {
                //then move player across network
                GameObject player = hitColliders[i].gameObject;
                Rpc_WakeUp(player, target);
                return;
            }
            i++;
        }
    }

    //Move player across network
    [ClientRpc]
    void Rpc_WakeUp(GameObject player, GameObject target)
    {
        //Calculate push vector
        targetDir = target.transform.position - this.transform.position;
        float step = rotationSpeed * Time.deltaTime;

        //Rotate towards a goal
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);

        //Set animation, sound and deactivate button
        // kickEffect.Play();

        animator.SetTrigger("TriggerAnger");
       // audioSync.PlaySound(3);
        wakeUp = false;
        GameObject.FindGameObjectWithTag("WakeUpButton").GetComponent<Image>().color = Color.white;

        /*
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.GetComponent<Main>().enabled)
                p.GetComponent<Main>().ListenerActionAvatar("Kick", player, target);
        }
        */
        Logger.LogAction("WakeUp", gameObject, player);
        /*
        //Log about kick
        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneInfo>().sessionID +
            "," + this.gameObject.GetComponent<UseTeleport>().playerID
            + "," + "WakeUp" + "," + this.name + "," + this.gameObject.transform.position + "," + player.name);
            */
        player.GetComponent<HybernationSystem>().hybernated = false;
            //CmdDisableHybernate(player);

       
    }

    
}
