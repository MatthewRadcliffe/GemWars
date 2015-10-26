using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class Player : NetworkBehaviour {
    [SyncVar]
    public int yellow = 0;
    [SyncVar]
    public int red = 0;
    [SyncVar]
    public int green = 0;
    [SyncVar]
    public int blue = 0;
    [SyncVar]
    public int purple = 0;
    [SyncVar]
    public float health = 1000;

    public int playerNum;
    private GameObject myHealthBar;
    private GameObject myResourcePanel;
    private GameObject opponent;
    private GameObject opponentsHealthBar;
    private UnitFactory factory;

    public void Start()
    {
        if(isLocalPlayer) {
            myResourcePanel = GameObject.Find("PlayersGems");

            if(isServer) {
                myHealthBar = GameObject.Find("Player1HealthBar");
                opponentsHealthBar = GameObject.Find("Player2HealthBar");
                playerNum = 1;
            }
            else {
                myHealthBar = GameObject.Find("Player2HealthBar");
                opponentsHealthBar = GameObject.Find("Player1HealthBar");
                playerNum = 2;
            }
        }
    }

    public void FixedUpdate ()
    {
        if (factory == null)
            factory = GameObject.Find("GameManager").GetComponent<UnitFactory>();

        if (!isLocalPlayer)
            return;

        updateUI();
	}

    [Command]
    public void Cmd_spawnUnit(string unit, float x, float y)
    {
        GameObject newUnit = (GameObject)Instantiate(Resources.Load(unit), new Vector2(x, y), Quaternion.identity);
        //newUnit.GetComponent<UnitBase>().controller = this;
        NetworkServer.Spawn(newUnit);
        factory.spawnUnit(newUnit, this);
    }

    [Command]
    public void Cmd_gainHealth(float amount)
    {
        if (!isServer)
            return;

        health += amount;
        if (health > 100)
            health = 100;
    }

    [Command]
    public void Cmd_loseHealth(float amount)
    {
        if (!isServer)
            return;

        health -= amount;
        if (health < 0)
            health = 0;
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

    public void updateUI()
    {
        myHealthBar.transform.FindChild("Health").GetComponent<Image>().fillAmount = health / 100f;
        myHealthBar.transform.FindChild("Health").transform.FindChild("Amount").GetComponent<Text>().text = health + " / 1000";
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

        opponentsHealthBar.transform.FindChild("Health").GetComponent<Image>().fillAmount = (opponent.GetComponent<Player>().health / 100);
        opponentsHealthBar.transform.FindChild("Health").transform.FindChild("Amount").GetComponent<Text>().text = opponent.GetComponent<Player>().health + " / 1000";

    }
}
