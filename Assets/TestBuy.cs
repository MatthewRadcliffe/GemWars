using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestBuy : NetworkBehaviour {

    Player myBuyer;
    public override void OnStartLocalPlayer()
    {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder)
            if (go.GetComponent<Player>().isLocalPlayer)
                myBuyer = go.GetComponent<Player>();
    }

    public void buy()
    {
        OnStartLocalPlayer();
        myBuyer.spendResource(Player.ResourceType.Yellow, 10);
    }

}
