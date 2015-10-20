using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ResourceGiver : MonoBehaviour {

    private Player player;

    public void findPlayer() {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder) {
            if (go.GetComponent<Player>().isLocalPlayer) {
                player = go.GetComponent<Player>();
            }
        }
    }

    public void giveResource(ResourceType type, int amount) {
        if (player == null) {
            findPlayer();
        }
        player.Cmd_gainResource(type, amount);
    }
}
