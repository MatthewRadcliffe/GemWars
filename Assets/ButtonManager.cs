using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ButtonManager : NetworkBehaviour {

    public GameObject[] units;
    public GameObject unitPanel; 
    public GameObject upgradePanel;
    private Player myPlayer;
    private float startingX;
    private float startingY;
    private float timer;
    private bool rematchAccepted = false, p1Rematch, p2Rematch;

    public void FixedUpdate() {
        //checkRematchStatus();
        if (myPlayer == null)
            findMyPlayer();

        if (myPlayer == null)
            return;

        timer -= Time.deltaTime;
        if(timer < 0) {
            timer = 0;
            setAllInteractable(true);
        }
        
        foreach (GameObject go in units)
            go.GetComponent<UnitBase>().setStats();

        for (int i = 0; i < units.Length; i++)
        {
            if(units[i] != null)
            {
                upgradePanel.transform.FindChild("UnitUpgrade" + (i + 1)).transform.FindChild("ResourceCost").transform.FindChild("Cost").GetComponent<Text>().text = "x " + units[i].GetComponent<UnitBase>().upgradeCost;
                unitPanel.transform.FindChild("UnitBuy" + (i + 1)).transform.FindChild("UnitLevelLabel").GetComponent<Text>().text = "Lvl " + units[i].GetComponent<UnitBase>().level;
                if (myPlayer.yellow < units[i].GetComponent<UnitBase>().upgradeCost)
                    upgradePanel.transform.FindChild("UnitUpgrade" + (i + 1)).GetComponent<Button>().interactable = false;
                else
                    upgradePanel.transform.FindChild("UnitUpgrade" + (i + 1)).GetComponent<Button>().interactable = true;

                if(myPlayer.yellow < units[i].GetComponent<UnitBase>().yellow || myPlayer.red < units[i].GetComponent<UnitBase>().red ||
                   myPlayer.green < units[i].GetComponent<UnitBase>().green || myPlayer.blue < units[i].GetComponent<UnitBase>().blue ||
                   myPlayer.purple < units[i].GetComponent<UnitBase>().purple)
                    unitPanel.transform.FindChild("UnitBuy" + (i + 1)).GetComponent<Button>().interactable = false;
                else
                    unitPanel.transform.FindChild("UnitBuy" + (i + 1)).GetComponent<Button>().interactable = true;
            }
        }

        //Hardcoded Stuff
        unitPanel.transform.FindChild("UnitBuy1").transform.FindChild("UnitCostPanel").transform.FindChild("ResourceCost").transform.FindChild("Cost").GetComponent<Text>().text = "x " + units[0].GetComponent<UnitBase>().green;
        unitPanel.transform.FindChild("UnitBuy2").transform.FindChild("UnitCostPanel").transform.FindChild("ResourceCost").transform.FindChild("Cost").GetComponent<Text>().text = "x " + units[1].GetComponent<UnitBase>().blue;
        unitPanel.transform.FindChild("UnitBuy3").transform.FindChild("UnitCostPanel").transform.FindChild("ResourceCost").transform.FindChild("Cost").GetComponent<Text>().text = "x " + units[2].GetComponent<UnitBase>().red;
        unitPanel.transform.FindChild("UnitBuy4").transform.FindChild("UnitCostPanel").transform.FindChild("ResourceCost").transform.FindChild("Cost").GetComponent<Text>().text = "x " + units[3].GetComponent<UnitBase>().purple;
        unitPanel.transform.FindChild("UnitBuy5").transform.FindChild("UnitCostPanel").transform.FindChild("ResourceCost").transform.FindChild("Cost").GetComponent<Text>().text = "x " + units[4].GetComponent<UnitBase>().yellow;
    }

    private void setAllInteractable(bool interactable) {
        for (int i = 0; i < units.Length; i++) {
            upgradePanel.transform.FindChild("UnitUpgrade" + (i + 1)).GetComponent<Button>().interactable = interactable;
            unitPanel.transform.FindChild("UnitBuy" + (i + 1)).GetComponent<Button>().interactable = interactable;
        }
    }

    private void checkRematchStatus() {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder) {
            Player temp = go.GetComponent<Player>();
            if(temp.isLocalPlayer && temp.getReset()) {
                if (!p1Rematch) {
                    p1Rematch = true;
                }
            } else {
                if(temp.getReset() && !p2Rematch) {
                    p2Rematch = true;
                }
            }
        }
        if(p1Rematch && p2Rematch) {
            rematchAccepted = true;
        }
    }

    public void endGame() {
        for(int i = 0; i < units.Length; i++) {
            upgradePanel.transform.FindChild("UnitUpgrade" + (i + 1)).GetComponent<Button>().interactable = false;
            unitPanel.transform.FindChild("UnitBuy" + (i + 1)).GetComponent<Button>().interactable = false;
        }
        //Button resetButton = GameObject.Find("WaitingPanel").transform.FindChild("ResetButton").GetComponent<Button>();
        //resetButton.transform.FindChild("Text").GetComponent<Text>().text = "Play Again?";
        //resetButton.gameObject.SetActive(true);
        //resetButton.interactable = true;
    }

    public void askForRematch() {
        foreach (GameObject go in units) {
            go.GetComponent<UnitBase>().level = 1;
        }
        GameObject.Find("WaitingPanel").transform.FindChild("Text").GetComponent<Text>().text = "Waiting for other player...";
        Button resetButton = GameObject.Find("WaitingPanel").transform.FindChild("ResetButton").GetComponent<Button>();
        resetButton.interactable = false;
        myPlayer.Cmd_setReset(true);
    }

    private void rematch()
    {
        if (myPlayer.playerNum == 1) {
            GameObject.Find("Board").GetComponent<BoardManager>().reset();
        }
    }

    public void upgrade(int unit)
    {
        if(myPlayer == null)
            findMyPlayer();

        units[unit].GetComponent<UnitBase>().upgrade();
        myPlayer.Cmd_spendResource(ResourceType.Yellow, units[unit].GetComponent<UnitBase>().upgradeCost);
    }

    public void purchaseUnit(int unit)
    {
        if (myPlayer == null) {
            findMyPlayer();
        }

        if (timer <= 0) {
            if (unit != 4) {
                myPlayer.Cmd_spawnUnit(units[unit].transform.name, units[unit].GetComponent<UnitBase>().level, startingX, startingY - .2f);
            }
            else {
                myPlayer.Cmd_spawnUnit(units[unit].transform.name, units[unit].GetComponent<UnitBase>().level, startingX, startingY + .4f);
            }

            myPlayer.Cmd_spendResource(ResourceType.Yellow, units[unit].GetComponent<UnitBase>().yellow);
            myPlayer.Cmd_spendResource(ResourceType.Red, units[unit].GetComponent<UnitBase>().red);
            myPlayer.Cmd_spendResource(ResourceType.Green, units[unit].GetComponent<UnitBase>().green);
            myPlayer.Cmd_spendResource(ResourceType.Blue, units[unit].GetComponent<UnitBase>().blue);
            myPlayer.Cmd_spendResource(ResourceType.Purple, units[unit].GetComponent<UnitBase>().purple);

            timer = 0.5f;

            setAllInteractable(false);
        }
    }


    private void findMyPlayer()
    {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder)
            if (go.GetComponent<Player>().isLocalPlayer)
                myPlayer = go.GetComponent<Player>();

        if(isServer)
        {
            startingX = GameObject.Find("Player1Base").transform.position.x;
            startingY = GameObject.Find("Player1Base").transform.position.y;
        }
        else {
            startingX = GameObject.Find("Player2Base").transform.position.x;
            startingY = GameObject.Find("Player2Base").transform.position.y;
        }
    }

}
