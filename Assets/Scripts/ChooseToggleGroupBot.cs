using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ChooseToggleGroupBot : MonoBehaviour {

    ToggleGroup toggleGroupInstance;
    string selectedName;
    public Button submitButton;
	bool choosed = false;

    public void OnClickChooseBot()
    {
        if (currentSelection != null)
        {
            switch (currentSelection.GetComponentInChildren<Text>().text)
            {
                case "A":
                    selectedName = "PlayerMod_A(Clone)";
                    break;
                case "B":
                    selectedName = "PlayerMod_B(Clone)";
                    break;
                case "C":
                    selectedName = "PlayerMod_C(Clone)";
                    break;
                case "D":
                    selectedName = "PlayerMod_D(Clone)";
                    break;
                default:
                    selectedName = "selectedNobody";
                    break;
            }

            Logger.LogAction("ConfirmedHuman " + selectedName, PlayerInfo.localPlayerGameObject, null);
        }

        toggleGroupInstance.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
		choosed = true;
    }

    public Toggle currentSelection
    {
        get
        {
            return toggleGroupInstance.ActiveToggles().FirstOrDefault();
       }
    }

	// Use this for initialization
	void Start () {
        toggleGroupInstance = GetComponent<ToggleGroup>();
        Text[] selectableNames = toggleGroupInstance.gameObject.GetComponentsInChildren<Text>();

        switch (PlayerInfo.localPlayerGameObject.name)
        {
            case "PlayerMod_A(Clone)":
                selectableNames[0].text = "B";
                selectableNames[1].text = "C";
                selectableNames[2].text = "D";
                break;
            case "PlayerMod_B(Clone)":
                selectableNames[0].text = "A";
                selectableNames[1].text = "C";
                selectableNames[2].text = "D";
                break;
            case "PlayerMod_C(Clone)":
                selectableNames[0].text = "A";
                selectableNames[1].text = "B";
                selectableNames[2].text = "D";
                break;
            case "PlayerMod_D(Clone)":
                selectableNames[0].text = "A";
                selectableNames[1].text = "B";
                selectableNames[2].text = "C";
                break;
        }

    }

	

	// Update is called once per frame
	void Update () {
       
		if (currentSelection != null && !choosed) {
			submitButton.gameObject.SetActive (true);
		} else {
			submitButton.gameObject.SetActive (false);
		}
            
	}
}
