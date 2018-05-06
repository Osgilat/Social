using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SpawnTeleport : NetworkBehaviour {

	public GameObject Platform_1;
	public GameObject Platform_2;
    public GameObject Generator;
    public GameObject GameManager;
    public GameObject UI_manager;

    GameObject p_1;
	GameObject p_2;
    GameObject gen;
    GameObject gameManager;
    GameObject ui_manager;


    public override void OnStartServer(){

       

        switch (SceneManager.GetActiveScene().name)
        {
            case "Teleports":
                p_1 = (GameObject)Instantiate(Platform_1, new Vector3(5.24f, 0.12f, 0.3527f), Quaternion.AngleAxis(270, Vector3.right));
                p_2 = (GameObject)Instantiate(Platform_2, new Vector3(-4.2f, 0.12f, 0.3527f), Quaternion.AngleAxis(270, Vector3.right));
                gameManager = (GameObject)Instantiate(GameManager);
                ui_manager = (GameObject)Instantiate(UI_manager);
                // gen = (GameObject) Instantiate(Generator, Generator.transform.position, Generator.transform.rotation);

                NetworkServer.Spawn(p_1);

                NetworkServer.Spawn(p_2);

                NetworkServer.Spawn(gameManager);

                NetworkServer.Spawn(ui_manager);

                //  NetworkServer.Spawn(gen);

                break;

		case "TeleportsML":
			p_1 = (GameObject)Instantiate(Platform_1, new Vector3(5.24f, 0.12f, 0.3527f), Quaternion.AngleAxis(270, Vector3.right));
			p_2 = (GameObject)Instantiate(Platform_2, new Vector3(-4.2f, 0.12f, 0.3527f), Quaternion.AngleAxis(270, Vector3.right));
			gameManager = (GameObject)Instantiate(GameManager);
			ui_manager = (GameObject)Instantiate(UI_manager);
                // gen = (GameObject) Instantiate(Generator, Generator.transform.position, Generator.transform.rotation);


                NetworkServer.Spawn(p_1);

			NetworkServer.Spawn(p_2);

			NetworkServer.Spawn(gameManager);

			NetworkServer.Spawn(ui_manager);

			//  NetworkServer.Spawn(gen);

			break;
                /*
            case "TeleportsVR":
                p_1 = (GameObject)Instantiate(Platform_1, new Vector3(5.24f, 0.12f, 0.3527f), Quaternion.AngleAxis(270, Vector3.right));
                p_2 = (GameObject)Instantiate(Platform_2, new Vector3(-4.2f, 0.12f, 0.3527f), Quaternion.AngleAxis(270, Vector3.right));
                ui = (GameObject)Instantiate(GameManager);
                // gen = (GameObject) Instantiate(Generator, Generator.transform.position, Generator.transform.rotation);



                NetworkServer.Spawn(ui);

                NetworkServer.Spawn(p_1);

                NetworkServer.Spawn(p_2);

                //  NetworkServer.Spawn(gen);

                break;
                */
            case "ThreeShooters":

                gameManager = (GameObject)Instantiate(GameManager);
                ui_manager = (GameObject)Instantiate(UI_manager);

                NetworkServer.Spawn(gameManager);

                NetworkServer.Spawn(ui_manager);

                break;

            case "Passengers":
                p_1 = (GameObject)Instantiate(Platform_1, new Vector3(5.24f, 0.12f, 0.3527f), Quaternion.AngleAxis(270, Vector3.right));
                p_2 = (GameObject)Instantiate(Platform_2, new Vector3(-4.2f, 0.12f, 0.3527f), Quaternion.AngleAxis(270, Vector3.right));
                gameManager = (GameObject)Instantiate(GameManager);
                ui_manager = (GameObject)Instantiate(UI_manager);
                gen = (GameObject) Instantiate(Generator, Generator.transform.position, Generator.transform.rotation);



                NetworkServer.Spawn(p_1);

                NetworkServer.Spawn(p_2);

                NetworkServer.Spawn(gameManager);

                NetworkServer.Spawn(ui_manager);

                NetworkServer.Spawn(gen);
                break;
               
            default:
                Debug.Log("WrongScene");
                break;
        }

       

	}

}
