using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class FixingGenerator : NetworkBehaviour {

    /*Network entities*/
    [SyncVar]
    private GameObject objectID;
    private NetworkIdentity objNetId;

    public static bool inGeneratorTrigger = false;
    public bool fixTrigger = false;
    public bool hybernateTrigger = false;
    public bool activeGeneratorTrigger = false;

    public static bool hasFixCharge = true;


    public void FixOnClick()
    {
        fixTrigger = true;
    }




    public void ActivateGeneratorOnClick()
    {

        if (gameObject.GetComponent<Text>().text !=
            PlayerInfo.localPlayerGameObject.GetComponent<Text>().text)
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
    }

}
