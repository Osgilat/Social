using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AndroidSceneManager : MonoBehaviour {

    public void LoadAR()
    {
        SceneManager.LoadScene("TeleportsAR");
    }

    public void LoadVR()
    {
        SceneManager.LoadScene("TeleportsVR");
    }

    public void LoadNET()
    {
        SceneManager.LoadScene("Lobby");
    }
}
