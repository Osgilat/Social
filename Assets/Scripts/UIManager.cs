using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager: NetworkBehaviour {

    //References to buttons
    public GameObject askButton;
    public GameObject kickButton;
    public GameObject shootButton;
    public GameObject activateButton;
    public GameObject takeOffButton;
    public GameObject saveButton;
    public GameObject fixButton;
    public GameObject healButton;
    public GameObject hybernateButton;
    public GameObject wakeUpButton;
    public GameObject targetButton;
    public GameObject thankButton;
    public GameObject activateGeneratorButton;
    public GameObject escapeButton;

    //References to HUD 
    public GameObject announcementText;
    public GameObject canvas;

    public void DisableAllButtons()
    {
        askButton.SetActive(false);
        kickButton.SetActive(false);
        activateButton.SetActive(false);
        takeOffButton.SetActive(false);
        saveButton.SetActive(false);
        fixButton.SetActive(false);
        healButton.SetActive(false);
        hybernateButton.SetActive(false);
        wakeUpButton.SetActive(false);
        targetButton.SetActive(false);
        thankButton.SetActive(false);
        activateGeneratorButton.SetActive(false);
        escapeButton.SetActive(false);

        saveButton.GetComponent<Image>().color = Color.white;

    }

    public void ChangeButtonsAfterFix()
    {
        askButton.SetActive(false);
        kickButton.SetActive(false);
        activateButton.SetActive(false);
        takeOffButton.SetActive(false);
        saveButton.SetActive(false);
        fixButton.SetActive(false);
        healButton.SetActive(false);
        hybernateButton.SetActive(true);
        wakeUpButton.SetActive(false);
        targetButton.SetActive(false);
        thankButton.SetActive(false);
        activateGeneratorButton.SetActive(true);
        escapeButton.SetActive(false);

    }

    public void ChangeTowerTP()
    {
        StartCoroutine(WaitForButtonActivationTP());

    }

    public void ChangeTowerPS()
    {
        StartCoroutine(WaitForButtonActivationPS());

    }

    public IEnumerator WaitForButtonActivationTP()
    {
        yield return new WaitForSeconds(1);


        askButton.SetActive(false);
        thankButton.SetActive(false);
        kickButton.SetActive(false);
        saveButton.SetActive(true);
        escapeButton.SetActive(true);

    }

    public IEnumerator WaitForButtonActivationPS()
    {
        yield return new WaitForSeconds(1);


        askButton.SetActive(false);
        thankButton.SetActive(false);
        kickButton.SetActive(false);
        saveButton.SetActive(true);
        fixButton.SetActive(true);
        wakeUpButton.SetActive(false);

    }

    public void ResetTeleportButtons()
    {
        askButton.SetActive(true);
        thankButton.SetActive(true);
        kickButton.SetActive(true);
    }

    public void ResetShootersButtons()
    {
        askButton.SetActive(true);
        thankButton.SetActive(true);
        kickButton.SetActive(true);

        shootButton.SetActive(true);
        targetButton.SetActive(true);
        healButton.SetActive(true);


    }

    public void ResetPassengersButtons()
    {
        askButton.SetActive(true);
        thankButton.SetActive(true);
        kickButton.SetActive(true);

        shootButton.SetActive(true);
        targetButton.SetActive(true);
        healButton.SetActive(true);

        wakeUpButton.SetActive(true);
    }

    public void DisableTeleportButtons()
    {
        takeOffButton.SetActive(false);
        activateButton.SetActive(false);
    }

    public void DisableAnnouncementText()
    {
        announcementText.SetActive(false);
    }

    public void SetActivateButton(bool active)
    {
        activateButton.SetActive(active);
    }

    public void SetTakeOffButton(bool active)
    {
        takeOffButton.SetActive(active);
    }

    public void RepaintKick()
    {
        kickButton.GetComponent<Image>().color = Color.white;
    }

    // Use this for initialization
    void Start ()
    {
        Invoke("InstantiateButtons", 2f);
    }

    
    /*
    private IEnumerator WaitForButtonInitialize()
    {
        yield return new WaitForSeconds(1);
    }
    */
    //Function for buttons and announcement
    void InstantiateButtons()
    {
   

        //Instantiate canvas and get references to it's buttons and announcement text
        Transform thisCanvas = Instantiate(canvas).transform;
        askButton = thisCanvas.GetChild(0).gameObject;
        kickButton = thisCanvas.GetChild(1).gameObject;
        activateButton = thisCanvas.GetChild(2).gameObject;
        takeOffButton = thisCanvas.GetChild(3).gameObject;
        saveButton = thisCanvas.GetChild(4).gameObject;
        fixButton = thisCanvas.GetChild(5).gameObject;
        announcementText = thisCanvas.GetChild(6).gameObject;
        shootButton = thisCanvas.GetChild(7).gameObject;
        healButton = thisCanvas.GetChild(8).gameObject;
        hybernateButton = thisCanvas.GetChild(9).gameObject;
        wakeUpButton = thisCanvas.GetChild(10).gameObject;
        targetButton = thisCanvas.GetChild(11).gameObject;
        thankButton = thisCanvas.GetChild(12).gameObject;
        activateGeneratorButton = thisCanvas.GetChild(13).gameObject;
        escapeButton = thisCanvas.GetChild(17).gameObject;

    
        
        switch (SceneManager.GetActiveScene().name)
        {
            case "Teleports":
                //Deactivate all buttons 
                activateButton.SetActive(false);
                takeOffButton.SetActive(false);
                saveButton.SetActive(false);
                fixButton.SetActive(false);
                hybernateButton.SetActive(false);
                wakeUpButton.SetActive(false);
                targetButton.SetActive(false);
                healButton.SetActive(false);
                shootButton.SetActive(false);
                activateGeneratorButton.SetActive(false);
                escapeButton.SetActive(false);

                break;

            case "TeleportsVR":
                //Deactivate all buttons 
                activateButton.SetActive(false);
                takeOffButton.SetActive(false);
                saveButton.SetActive(false);
                fixButton.SetActive(false);
                hybernateButton.SetActive(false);
                wakeUpButton.SetActive(false);
                targetButton.SetActive(false);
                healButton.SetActive(false);
                shootButton.SetActive(false);
                activateGeneratorButton.SetActive(false);
                escapeButton.SetActive(false);

                break;

            case "ThreeShooters":


                //Deactivate all buttons 
                activateButton.SetActive(false);
                takeOffButton.SetActive(false);
                saveButton.SetActive(false);
                fixButton.SetActive(false);
                hybernateButton.SetActive(false);
                wakeUpButton.SetActive(false);
                escapeButton.SetActive(false);


                shootButton.SetActive(false);
                activateGeneratorButton.SetActive(false);
                break;

            case "Passengers":
                //Deactivate all buttons 
                activateButton.SetActive(false);
                takeOffButton.SetActive(false);
                saveButton.SetActive(false);
                fixButton.SetActive(false);
                hybernateButton.SetActive(false);
                escapeButton.SetActive(false);



                shootButton.SetActive(false);

                break;
            default:

                Debug.Log("WrongScene");
                break;
        }





        announcementText.GetComponent<Text>().text = "You are player " + PlayerInfo.localPlayerGameObject.
            GetComponent<PlayerInfo>().playerID;

        //Set announcement active for a first round
        announcementText.SetActive(true);
        
    }

    public static Hashtable buttons;

    public void ResetUI(GameObject resetForButton)
    {

        buttons = new Hashtable();

        buttons.Add(askButton, PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().askTrigger);
        buttons.Add(kickButton, PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().kickTrigger);
        buttons.Add(healButton, PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().healTrigger);
        buttons.Add(wakeUpButton, PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().wakeTrigger);
        buttons.Add(targetButton, PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().targetTrigger);
        buttons.Add(thankButton, PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().thankTrigger);
        buttons.Add(saveButton, PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().saveTrigger);


        buttons.Remove(resetForButton);

        List<string> keys = new List<string>();

        foreach (DictionaryEntry entry in buttons)
        {
            ((GameObject)entry.Key).GetComponent<Button>().GetComponent<Image>().color = Color.white;
            keys.Add(entry.Key.ToString());
        }

        foreach (string key in keys)
        {
            switch (key.ToString())
            {
                case "Ask (UnityEngine.GameObject)": PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().askTrigger = false; break;
                case "Kick (UnityEngine.GameObject)": PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().kickTrigger= false; break;
                case "Heal (UnityEngine.GameObject)": PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().healTrigger = false; break;
                case "Target (UnityEngine.GameObject)": PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().targetTrigger = false; break;
                case "WakeUp (UnityEngine.GameObject)": PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().wakeTrigger = false; break;
                case "Thank (UnityEngine.GameObject)": PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().thankTrigger = false; break;
                case "Save (UnityEngine.GameObject)": PlayerInfo.localPlayerGameObject.GetComponent<PlayerSkills>().saveTrigger = false; break;

            }
        }

    }

    
    // Update is called once per frame
    void Update ()
    {
        

        if (saveButton.activeSelf)
        {
            announcementText.SetActive(true);
            announcementText.GetComponent<Text>().text =
                PlayerInfo.localPlayerGameObject.GetComponent<PlayerInfo>().playerToTarget1.GetComponent<Text>().text + " 1 \n"
                 + PlayerInfo.localPlayerGameObject.GetComponent<PlayerInfo>().playerToTarget2.GetComponent<Text>().text + " 2 \n"
                 + "Kick -> Escape \n";
        }
        else
        {
            announcementText.GetComponent<Text>().text = "";
        }
    }
    
}
