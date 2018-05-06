using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : NetworkBehaviour
{

    //PlayerID for identification between sessions
    [SyncVar]
    public string playerID = "noPlayerID";
    public static GameObject localPlayerGameObject;
    public Material localPlayerMaterial;
    public GameObject objectToPaint;
    public static UIManager UIManager;

    public GameObject playerToTarget1 = null;
    public GameObject playerToTarget2 = null;

    void Awake()
    {
        localPlayerGameObject = gameObject;
    }

    // Use this for initialization
    void Start ()
    {

        string temp = gameObject.name.Remove(11);
        playerID = temp.Replace("Mod_", " ");

        localPlayerGameObject = gameObject;

        Invoke("SearchForOtherPlayers", 2f);
    }

    public override void OnStartLocalPlayer()
    {
        localPlayerGameObject = gameObject;
        objectToPaint.GetComponent<MeshRenderer>().material = localPlayerMaterial;
    }

    public bool IsTrueLocalPlayer()
    {
        
        if (gameObject.GetComponent<Text>().text !=
            localPlayerGameObject.GetComponent<Text>().text
            || gameObject.GetComponent<AI_behaviour>().enabled)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void SearchForOtherPlayers()
    {
        UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player != gameObject)
            {
                if (playerToTarget1 == null)
                {
                    playerToTarget1 = player;
                }

                if (playerToTarget1 != null)
                {
                    playerToTarget2 = player;
                }
            }
        }
    }

}
