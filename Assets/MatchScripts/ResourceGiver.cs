using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ResourceGiver : NetworkBehaviour {

    private Player player;
	// Use this for initialization
	void Start () {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder) {
            if (go.GetComponent<Player>().isLocalPlayer) {
                player = go.GetComponent<Player>();
            }
        }

    }

    public void giveResource(ResourceType type, int amount) {
        player.Cmd_gainResource(type, amount);
    }
}
