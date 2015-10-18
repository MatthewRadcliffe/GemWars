using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class Player : NetworkBehaviour {
    [SyncVar]
    private int yellow = 0;
    [SyncVar]
    private int red = 0;
    [SyncVar]
    private int green = 0;
    [SyncVar]
    private int blue = 0;
    [SyncVar]
    private int purple = 0;
    private GameObject myResourcePanel;
    private GameObject opponentsResourcePanel;
    private GameObject opponent;

    public void Start()
    {
        if(isLocalPlayer)
        {
            myResourcePanel = GameObject.Find("PlayersGems");
            opponentsResourcePanel = GameObject.Find("OpponentsGems");
        }
    }

    public void FixedUpdate ()
    {
        updateResources();
	}

    [Command]
    public void Cmd_gainResource(ResourceType resource, int amount)
    {
        if (!isServer)
            return;

        switch (resource)
        {
            case ResourceType.Yellow:
                yellow += amount;
                break;
            case ResourceType.Red:
                red += amount;
                break;
            case ResourceType.Green:
                green += amount;
                break;
            case ResourceType.Blue:
                blue += amount;
                break;
            case ResourceType.Purple:
                purple += amount;
                break;
        }
    }

    [Command]
    public void Cmd_spendResource(ResourceType resource, int amount)
    {
        if (!isServer)
            return;

        switch (resource)
        {
            case ResourceType.Yellow:
                yellow -= amount;
                break;
            case ResourceType.Red:
                red -= amount;
                break;
            case ResourceType.Green:
                green -= amount;
                break;
            case ResourceType.Blue:
                blue -= amount;
                break;
            case ResourceType.Purple:
                purple -= amount;
                break;
        }
    }

    public void updateResources()
    {
        if (!isLocalPlayer)
            return;

        myResourcePanel.transform.FindChild("YellowGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + yellow;
        myResourcePanel.transform.FindChild("RedGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + red;
        myResourcePanel.transform.FindChild("GreenGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + green;
        myResourcePanel.transform.FindChild("BlueGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + blue;
        myResourcePanel.transform.FindChild("PurpleGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + purple;

        if (opponent == null) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject go in players)
                if (!go.GetComponent<Player>().isLocalPlayer)
                    opponent = go;
            return;
        }

        opponentsResourcePanel.transform.FindChild("YellowGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + opponent.GetComponent<Player>().yellow;
        opponentsResourcePanel.transform.FindChild("RedGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + opponent.GetComponent<Player>().red;
        opponentsResourcePanel.transform.FindChild("GreenGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + opponent.GetComponent<Player>().green;
        opponentsResourcePanel.transform.FindChild("BlueGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + opponent.GetComponent<Player>().blue;
        opponentsResourcePanel.transform.FindChild("PurpleGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + opponent.GetComponent<Player>().purple;

    }
}
