using UnityEngine;
using UnityEngine.Networking;
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
    [SyncVar]
    private bool reset = false;
    [SyncVar]
    private bool gameOver = false;

    public int playerNum;
    private GameObject myBase, opponentBase;
    private GameObject myHealthBar;
    private GameObject myResourcePanel;
    private GameObject opponent;
    private GameObject opponentsHealthBar;
    private UnitFactory factory;

    public void Start() {
        if(isLocalPlayer) {
            myResourcePanel = GameObject.Find("PlayersGems");

            if(isServer) {
                myHealthBar = GameObject.Find("Player1HealthBar");
                opponentsHealthBar = GameObject.Find("Player2HealthBar");
                playerNum = 1;
                myBase = GameObject.Find("Player1Base");
                opponentBase = GameObject.Find("Player2Base");
            }
            else {
                myHealthBar = GameObject.Find("Player2HealthBar");
                opponentsHealthBar = GameObject.Find("Player1HealthBar");
                playerNum = 2;
                myBase = GameObject.Find("Player2Base");
                opponentBase = GameObject.Find("Player1Base");
            }
        }
    }

    public void FixedUpdate () {
        if (factory == null)
            factory = GameObject.Find("GameManager").GetComponent<UnitFactory>();
        
        if (!isLocalPlayer)
            return;

        if (!gameOver) {
            string endText = "";
            if (health <= 0) { health = 0;
                endText = "Game Over! \n You Lose.";
            } else if (opponent != null && opponent.GetComponent<Player>().health <= 0) {
                opponent.GetComponent<Player>().health = 0;
                endText = "Game Over! \n You Win.";
            }

            if (endText.Length > 0) {
                GameObject.Find("WaitingPanel").transform.position = new Vector3();
                GameObject.Find("WaitingPanel").transform.FindChild("Text").GetComponent<Text>().text = endText;
                gameOver = true;

                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Tile")) {
                    Destroy(go);
                }
            }
            updateUI();
        }
	}

    public bool getRematchStatus() {
        return reset;
    }

    [Command]
     public void Cmd_requestRematch() {
        if(!isServer) {
            return;
        }
        reset = true;
    }
    
    [Command]
    public void Cmd_resetPlayer() {
        if(!isServer) {
            return;
        }
        gameOver = false;
        reset = false;
        yellow = 0;
        red = 0;
        green = 0;
        blue = 0;
        purple = 0;
        health = 1000;
        GameObject inputBoard = GameObject.Find("Board");
        PlayerInput input = inputBoard.GetComponent<PlayerInput>();
        input.reset();
    }

    [Command]
    public void Cmd_receiveInput(bool leftClick, Vector3 mousePosition)  {
        GameObject inputBoard = GameObject.Find("Board");
        PlayerInput input = inputBoard.GetComponent<PlayerInput>();
        if (leftClick) {
            input.onLeftClick(playerNum, mousePosition);
        }
        else {
            input.onRightClick(playerNum);
        }
    }
    
    [Command]
    public void Cmd_spawnUnit(string unit, int level, float x, float y) {
        GameObject newUnit = (GameObject)Instantiate(Resources.Load(unit), new Vector2(x, y), Quaternion.identity);
        newUnit.GetComponent<UnitBase>().level = level;
        newUnit.GetComponent<UnitBase>().setStats();
        //newUnit.GetComponent<UnitBase>().controller = this;
        NetworkServer.Spawn(newUnit);
        factory.spawnUnit(newUnit, this);
    }

    [Command]
    public void Cmd_gainHealth(float amount) {
        if (!isServer)
            return;

        health += amount;
        if (health > 1000)
            health = 1000;
    }

    [Command]
    public void Cmd_loseHealth(float amount) {
        if (!isServer)
            return;

        health -= amount;
        if (health < 0)
            health = 0;
    }

    [Command]
    public void Cmd_gainResource(ResourceType resource, int amount) {
        if (!isServer)
            return;

        switch (resource) {
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
    public void Cmd_spendResource(ResourceType resource, int amount) {
        if (!isServer)
            return;

        switch (resource) {
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
        myHealthBar.transform.FindChild("Health").GetComponent<Image>().fillAmount = health / 1000f;
        myHealthBar.transform.FindChild("Health").transform.FindChild("Amount").GetComponent<Text>().text = health + " / 1000";

        myBase.GetComponent<CastleUpdater>().updateSprite(health);

        myResourcePanel.transform.FindChild("YellowGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + yellow;
        myResourcePanel.transform.FindChild("RedGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + red;
        myResourcePanel.transform.FindChild("GreenGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + green;
        myResourcePanel.transform.FindChild("BlueGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + blue;
        myResourcePanel.transform.FindChild("PurpleGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + purple;

        if (opponent == null) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject go in players) {
                if (!go.GetComponent<Player>().isLocalPlayer) {
                    opponent = go;
                }
            }
            return;
        }
        opponentsHealthBar.transform.FindChild("Health").GetComponent<Image>().fillAmount = (opponent.GetComponent<Player>().health / 1000);
        opponentsHealthBar.transform.FindChild("Health").transform.FindChild("Amount").GetComponent<Text>().text = opponent.GetComponent<Player>().health + " / 1000";

        opponentBase.GetComponent<CastleUpdater>().updateSprite(opponent.GetComponent<Player>().health);
    }
}
