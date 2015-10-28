using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LocalInput : NetworkBehaviour {
    //print("PLAYER " + temp.playerNum + ": " + Input.mousePosition.x + ", " + Input.mousePosition.y);
    private Player myPlayer;

    private void setPlayer() {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder) {
            if (go.GetComponent<Player>().isLocalPlayer) {
                myPlayer = go.GetComponent<Player>();
            }
        }
    }

    void Update() {
        if(myPlayer == null) {
            setPlayer();
        }

        if(myPlayer == null) {
            return;
        }

        if (Input.GetMouseButton(0)) {
            myPlayer.Cmd_receiveInput(true, Input.mousePosition);
        }
        else if (Input.GetMouseButton(1)) {
            myPlayer.Cmd_receiveInput(false, Input.mousePosition);
        }
    }
}
