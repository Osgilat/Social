using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class FixAbility : NetworkBehaviour {

    /*Network entities*/
    [SyncVar]
    private GameObject objectID;
    private NetworkIdentity objNetId;

    public static bool inGeneratorTrigger = false;
    public static bool fixTrigger = false;
    public static bool activeGeneratorTrigger = false;

    public static bool hasFixCharge = true;


    void FixedUpdate()
    {
        //Control only by local player
        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer()
            || !UseTeleport.initializeTrigger)
        {
            return;

        }

        if (fixTrigger)
        {
            fixTrigger = false;
            CmdFixGenerator(GameObject.FindGameObjectWithTag("Generator"));
        }

        if (activeGeneratorTrigger)
        {
            activeGeneratorTrigger = false;
            CmdActivateGenerator(GameObject.FindGameObjectWithTag("Generator"));
        }
        
    }

    public void FixOnClick()
    {
        if (!GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }


        fixTrigger = true;
        PlayerInfo.UIManager.fixButton.SetActive(false);
        //CmdFixGenerator(GameObject.FindGameObjectWithTag("Generator"));
    }

    public void AI_Fix()
    {
        CmdFixGenerator(GameObject.FindGameObjectWithTag("Generator"));
    }


    public void ActivateGeneratorOnClick()
    {

        if (!gameObject.GetComponent<PlayerInfo>().IsTrueLocalPlayer())
        {
            return;
        }

        activeGeneratorTrigger = true;


    }

    [Command]
    void CmdFixGenerator(GameObject obj)
    {

        objNetId = obj.GetComponent<NetworkIdentity>();
        objNetId.AssignClientAuthority(connectionToClient);
        RpcFixGenerator(obj);
        objNetId.RemoveClientAuthority(connectionToClient);
    }

    //Rpc other teleport
    [ClientRpc]
    void RpcFixGenerator(GameObject obj)
    {
        
        obj.GetComponent<Generator>().repairPoints -= 1;
        
    }

    [Command]
    void CmdActivateGenerator(GameObject obj)
    {

        objNetId = obj.GetComponent<NetworkIdentity>();
        objNetId.AssignClientAuthority(connectionToClient);
        RpcActivateGenerator(obj);
        objNetId.RemoveClientAuthority(connectionToClient);
    }

    //Rpc other teleport
    [ClientRpc]
    void RpcActivateGenerator(GameObject obj)
    {

        obj.GetComponent<Generator>().active = true;


        PlayerInfo.UIManager.activateButton.SetActive(false);
    }

}
