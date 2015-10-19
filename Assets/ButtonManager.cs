using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class ButtonManager : NetworkBehaviour {

    public GameObject[] units;
    public GameObject unitPanel; 
    public GameObject upgradePanel;
    private Player myPlayer;

    public void Start()
    {

    }

    public void FixedUpdate()
    {
        if (myPlayer == null)
            findMyPlayer();

        for (int i = 0; i < units.Length; i++)
        {
            if(units[i] != null)
            {
                upgradePanel.transform.FindChild("UnitUpgrade" + (i + 1)).transform.FindChild("ResourceCost").transform.FindChild("Cost").GetComponent<Text>().text = "x " + units[i].GetComponent<UnitBase>().upgradeCost;
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
    }

    public void upgrade(int unit)
    {
        if(myPlayer == null)
            findMyPlayer();

        myPlayer.Cmd_spendResource(ResourceType.Yellow, units[unit].GetComponent<UnitBase>().upgradeCost);
    }

    public void purchaseUnit(int unit)
    {
        if (myPlayer == null)
            findMyPlayer();

        myPlayer.Cmd_spendResource(ResourceType.Yellow, units[unit].GetComponent<UnitBase>().yellow);
        myPlayer.Cmd_spendResource(ResourceType.Red, units[unit].GetComponent<UnitBase>().red);
        myPlayer.Cmd_spendResource(ResourceType.Green, units[unit].GetComponent<UnitBase>().green);
        myPlayer.Cmd_spendResource(ResourceType.Blue, units[unit].GetComponent<UnitBase>().blue);
        myPlayer.Cmd_spendResource(ResourceType.Purple, units[unit].GetComponent<UnitBase>().purple);
    }

    private void findMyPlayer()
    {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder)
            if (go.GetComponent<Player>().isLocalPlayer)
                myPlayer = go.GetComponent<Player>();
    }

}
