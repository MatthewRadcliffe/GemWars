using UnityEngine;

public class ResourceGiver : MonoBehaviour {
    private Player p1, p2;

    private void setPlayers() {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder) {
            if (go.GetComponent<Player>().playerNum == 1) {
                p1 = go.GetComponent<Player>();
            }
            if (go.GetComponent<Player>().playerNum == 2) {
                p2 = go.GetComponent<Player>();
            }
        }
    }

    public void giveResource(int playerNum, ResourceType type, int amount) {
        if(p1 == null) {
            setPlayers();
        }
        Player toGive = (playerNum == 1) ? p1 : p2;
        toGive.Cmd_gainResource(type, amount);
    }
}
