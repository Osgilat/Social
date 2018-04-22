using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;


public class SessionData : MonoBehaviour {

    public ToggleGroup sessionToggleGroup;
    public Dropdown aiDropDown;

   

    public string selectedScene = "";
    public string selectedAiCount = "";

    private string tempScene = "";

    public GameObject menu;

   

    // Update is called once per frame
    void Update ()
    {

        
        bool maySaveSceneName = sessionToggleGroup != null &&
            sessionToggleGroup.ActiveToggles().FirstOrDefault() != null;

        if (maySaveSceneName)
        {
            selectedScene = sessionToggleGroup
                .ActiveToggles().FirstOrDefault()
                .GetComponentInChildren<Text>().text;
        }

        bool maySaveAiCount = aiDropDown != null &&
            tempScene != selectedScene;

        if (maySaveAiCount)
        {
            ChangeDropdownOptions();
            
        }
        if (aiDropDown != null)
        { 
            selectedAiCount = aiDropDown.captionText.text;
        }

        tempScene = selectedScene;
        

    }

    private void ChangeDropdownOptions()
    {
        List<string> dropOptions = new List<string>();

        switch (selectedScene)
        {
            case "TELEPORTS":

                
                dropOptions =
                    new List<string> { "0", "1", "2", "3" };
                
                break;

            case "SHOOTERS":

                dropOptions =
                    new List<string> { "0", "1", "2", "3" };

                break;

            case "PASSENGERS":

                dropOptions =
                    new List<string> { "0", "1", "2", "3", "4" };

                break;
            default:
                Debug.Log("WrongScene");
                break;
        }

        aiDropDown.ClearOptions();
        aiDropDown.AddOptions(dropOptions);
    }
}
