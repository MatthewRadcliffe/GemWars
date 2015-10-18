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
        if(myBuyer == null)
            OnStartLocalPlayer();
        myBuyer.Cmd_spendResource(ResourceType.Yellow, 10);
    }

}
