using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HealAbility : NetworkBehaviour
{
    public bool healOnceBool = false;
    private const float playerSpeed = 1.5f;
    private const float timeForHeal = 7.0f;
    public bool healTrigger = false;
    public GameObject targetToHeal = null;



    public void ClickToHeal()
    {

        if (!gameObject.GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }


        PlayerInfo.UIManager.ResetUI(PlayerInfo.UIManager.healButton);
        if (!healTrigger)
        {
            healTrigger = true;
            GameObject.FindGameObjectWithTag("HealButton").GetComponent<Image>().color = Color.clear;
            
        }

        else
        {
            healTrigger = false;
            GameObject.FindGameObjectWithTag("HealButton").GetComponent<Image>().color = Color.white;
        }

    }

    public void AI_Heal(GameObject playerToHeal)
    {
        healTrigger = true;
        CmdParticles(playerToHeal);
    }

    void FixedUpdate()
    {
        //Control only by local player
        if (!isLocalPlayer)
        {
            return;
        }
        

        if (healOnceBool)
        {
            PlayerInfo.UIManager.healButton.SetActive(false);
        }

        //if left clicked
        if (Input.GetMouseButtonDown(0))
        {
            //Clicked place
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {

                //if greetButton active and clicked on a player
                if (healTrigger && hit.transform.CompareTag("Player")
                    && hit.transform.gameObject.GetComponent<ShootAbility>().stunned
                    && hit.transform.gameObject != gameObject
                    && !gameObject.GetComponent<ShootAbility>().stunned
                    && !healOnceBool)
                {
                    healOnceBool = true;

                    CmdParticles(hit.transform.gameObject);
                }
            }
        }
    }

    [Command]
    void CmdParticles(GameObject targetPlayer)
    {

        //Calling on clients
        targetPlayer.GetComponent<ShootAbility>().stunned = false;
        targetPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        targetPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = playerSpeed;
        targetToHeal = targetPlayer;
        targetPlayer.GetComponentInChildren<Animator>().SetBool("Died", false);
        healTrigger = false;
        RpcParticles(targetPlayer);
    }

    //For each client make event active
    [ClientRpc]
    void RpcParticles(GameObject targetPlayer)
    {
        Logger.LogAction("Heal", gameObject, targetPlayer);

        targetPlayer.GetComponent<ShootAbility>().stunned = false;
        targetPlayer.GetComponent<ShootAbility>().timeForHeal = timeForHeal;
        targetPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        targetPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = playerSpeed;
        targetToHeal = targetPlayer;
        targetPlayer.GetComponentInChildren<Animator>().SetBool("Died", false);
        healTrigger = false;
    }

}
