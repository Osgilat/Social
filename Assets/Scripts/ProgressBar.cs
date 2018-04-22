using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

    Image foregroundImage;
    public Image playerImageA;
    public Image playerImageB;
    public Image playerImageC;
    public Image playerImageD;
    private GameObject playerA;
    private GameObject playerB;
    private GameObject playerC;
    private GameObject playerD;
    GameObject gameManager;
    private bool initBool = false;

    void Start()
    {
        Invoke("Initialization", 15f);

        foregroundImage = gameObject.GetComponent<Image>();
    }

    void Initialization()
    {

        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.name == "PlayerMod_A(Clone)")
            {
                playerA = player;
            }
            else if (player.name == "PlayerMod_B(Clone)")
            {
                playerB = player;

            }
            else if (player.name == "PlayerMod_C(Clone)")
            {
                playerC = player;
            }
            else if (player.name == "PlayerMod_D(Clone)")
            {
                playerD = player;
            }
        }

        initBool = true;

    }
       

    private void Update()
    {
        
        if (!initBool)
        {
            return;
        }


        foregroundImage.fillAmount = gameManager.GetComponent<GameManagerTeleports>().timeLeftInGame / 600;

		if(playerA != null) playerImageA.fillAmount = playerA.GetComponent<UseTeleport>().timesSaved * 0.02f;
        if(playerB != null) playerImageB.fillAmount = playerB.GetComponent<UseTeleport>().timesSaved * 0.02f;
        if(playerC != null) playerImageC.fillAmount = playerC.GetComponent<UseTeleport>().timesSaved * 0.02f;
        if(playerD != null) playerImageD.fillAmount = playerD.GetComponent<UseTeleport>().timesSaved * 0.02f;
        
    }
    
}


