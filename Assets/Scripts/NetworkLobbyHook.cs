using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class NetworkLobbyHook : LobbyHook {

    //Used to sync playerID from lobby
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, 
        GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();

        gamePlayer.GetComponent<PlayerInfo>().playerID = lobby.playerName;
    }
    
}
