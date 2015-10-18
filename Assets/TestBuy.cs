using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestBuy : NetworkBehaviour {

    Player myBuyer;

    public void buy()
    {
        if(myBuyer == null)
            findMyPlayer();
        myBuyer.Cmd_spendResource(ResourceType.Yellow, 10);
    }

    public void loseHP()
    {
        if (myBuyer == null)
            findMyPlayer();
        myBuyer.Cmd_loseHealth(13.5f);
    }

    public void findMyPlayer()
    {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder)
            if (go.GetComponent<Player>().isLocalPlayer)
                myBuyer = go.GetComponent<Player>();
    }

}
