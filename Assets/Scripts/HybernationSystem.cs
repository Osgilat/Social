using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HybernationSystem : NetworkBehaviour
{
    //Network entities
    [SyncVar]
    private GameObject objectID;
    private NetworkIdentity objNetId;

    public GameObject hybernationSphere;
    public ParticleSystem hybernationEffect;

    private static bool hybernateTrigger;

    //to check points
    [SyncVar]
    public int hybernationPoints = 0;


    [SyncVar]
    public bool hybernated = false;


    public bool isHybernated()
    {
        return hybernated;
    }

    /*
    [Command]
    public void CmdRandomHybernation()
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.GetComponent<HybernationSystem>().hybernated = true;
        }

        int randPlayer = Random.Range(0, players.Length - 1);
        RpcRandomHybernation(randPlayer, players);
        
    }

    [ClientRpc]
    public void RpcRandomHybernation(int randPlayer, GameObject[] players)
    {
        players[randPlayer].GetComponent<HybernationSystem>().hybernated = false;
    }
    */

    private void Update()
    {
        if (!GetComponent<UseTeleport>().passengersScene)
        {
            return;
        }

        if (hybernated)
        {
            EnableHybernation();
        }
        else
        {
            DisableHybernation();
        }

        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }
        

        if(GameObject.FindGameObjectWithTag("Generator")
            .GetComponent<Generator>().repairPoints <= 0 
            && FixAbility.inGeneratorTrigger)
        {
            PlayerInfo.UIManager.ChangeButtonsAfterFix();
        }

        



        HandleHybernation();

    }

    private void HandleHybernation()
    {


        if (hybernateTrigger)
        {
            if (gameObject.GetComponent<HybernationSystem>().hybernationEffect.isStopped)
                gameObject.GetComponent<HybernationSystem>().hybernationEffect.Play();
           
            //if clicked 
            if (Input.GetMouseButton(0))
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Hybernate " + hit.transform.gameObject);
                    //if hitted player
                    if (hit.transform.CompareTag("Player") && hit.transform.gameObject != gameObject)
                    {

                        //Transform hitted player
                        GameObject[] spawnPoints = GetSpawnPoints();

                        GetComponent<UseTeleport>().RespawnPlayers(hit.transform.gameObject, spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.position,
                            spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.rotation, false);

                        
                        //hit.transform.gameObject.GetComponent<HybernationSystem>().
                        CmdHybernate(hit.transform.gameObject);

                        gameObject.GetComponent<HybernationSystem>().hybernationEffect.Stop();

                        PlayerInfo.UIManager.thankButton.GetComponent<Image>().color = Color.white;
                        //PlayerInfo.UIManager.DisableAllButtons();
                        hybernateTrigger = false;
                    }
                }
            }
        }
        else
        {
            gameObject.GetComponent<HybernationSystem>().hybernationEffect.Stop();
        }
    }


    [Command]
    public void CmdHybernate(GameObject obj)
    {

        objNetId = obj.GetComponent<NetworkIdentity>();
        objNetId.AssignClientAuthority(connectionToClient);
        RpcHybernate(obj);
        objNetId.RemoveClientAuthority(connectionToClient);

    }


    [ClientRpc]
    public void RpcHybernate(GameObject obj)
    {
        obj.GetComponent<HybernationSystem>().hybernated = true;
        Logger.LogAction("Hybernate", gameObject, obj);

    }

    [Command]
    public void CmdDisableHybernate(GameObject obj)
    {
        objNetId = obj.GetComponent<NetworkIdentity>();
        objNetId.AssignClientAuthority(connectionToClient);
        RpcDisableHybernate(obj);
        objNetId.RemoveClientAuthority(connectionToClient);
    }

    [ClientRpc]
    public void RpcDisableHybernate(GameObject obj)
    {
        obj.GetComponent<HybernationSystem>().hybernated = false;
    }

    private void EnableHybernation()
    {
        gameObject.GetComponent<UseTeleport>().enabled = false;
        gameObject.GetComponent<ShootAbility>().enabled = false;
        gameObject.GetComponent<PlayerActor>().enabled = false;
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        gameObject.GetComponent<PushAbility>().enabled = false;
        gameObject.GetComponent<HealAbility>().enabled = false;
        hybernationSphere.GetComponent<MeshRenderer>().enabled = true;
    }

    private void DisableHybernation()
    {
        gameObject.GetComponent<UseTeleport>().enabled = true;
        gameObject.GetComponent<ShootAbility>().enabled = true;
        gameObject.GetComponent<PlayerActor>().enabled = true;
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        gameObject.GetComponent<PushAbility>().enabled = true;
        gameObject.GetComponent<HealAbility>().enabled = true;
        hybernationSphere.GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnHybernateClick()
    {

        if (!gameObject.GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        PlayerInfo.UIManager.ResetUI(PlayerInfo.UIManager.hybernateButton);


        if (!hybernateTrigger)
        {
            hybernateTrigger = true;
            PlayerInfo.UIManager.hybernateButton.GetComponent<Image>().color = Color.clear;

        }
        else
        {
            hybernateTrigger = false;
            PlayerInfo.UIManager.thankButton.GetComponent<Image>().color = Color.white;
        }
    }

    private GameObject[] GetSpawnPoints()
    {
        GameObject[] spawnPoints = null;

        switch (GameObject.FindGameObjectWithTag("MainCamera").
            GetComponent<SceneInfo>().sceneName)
        {
            case "Teleports":

                spawnPoints = GameObject.FindGameObjectWithTag("GameManager")
                    .GetComponent<GameManagerTeleports>().spawnPoints;

                break;
                
            case "ThreeShooters":

                spawnPoints = GameObject.FindGameObjectWithTag("GameManager")
                    .GetComponent<GameManagerTeleports>().spawnPoints;

                break;
            case "Passengers":

                spawnPoints = GameObject.FindGameObjectWithTag("GameManager")
                    .GetComponent<GameManagerTeleports>().spawnPoints;

                break;
                
        }

        return spawnPoints;
    }

    public void AI_Hybernate(GameObject target)
    {
        Logger.LogAction("Hybernate", gameObject, target);

        /*
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<Behaviour>().enabled)
                player.GetComponent<Behaviour>().ListenerActionAvatar("Hybernated", this.gameObject, target.transform.gameObject);
        }
        */
        //Transform hitted player
        GameObject[] spawnPoints = GetSpawnPoints();

        GetComponent<UseTeleport>().RespawnPlayers(target.transform.gameObject, spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.position,
            spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.rotation, false);

        //hit.transform.gameObject.GetComponent<HybernationSystem>().
        CmdHybernate(target.transform.gameObject);
    }

}
