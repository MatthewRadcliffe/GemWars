using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public enum ResourceType
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple
    }

    private int yellow = 0;
    private int red = 0;
    private int green = 0;
    private int blue = 0;
    private int purple = 0;
    private GameObject resourcePanel;

    public override void OnStartLocalPlayer()
    {
        resourcePanel = GameObject.Find("PlayersGems");
        updateResources();
    }

    // Update is called once per frame
    void Update () {
	
	}


    public void gainResource(ResourceType resource, int amount)
    {
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
        updateResources();
    }

    public void spendResource(ResourceType resource, int amount)
    {
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
        updateResources();
    }

    public void updateResources()
    {
        resourcePanel.transform.FindChild("YellowGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + yellow;
        resourcePanel.transform.FindChild("RedGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + red;
        resourcePanel.transform.FindChild("GreenGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + green;
        resourcePanel.transform.FindChild("BlueGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + blue;
        resourcePanel.transform.FindChild("PurpleGem").transform.FindChild("GemCount").GetComponent<Text>().text = "x " + purple;
    }
}
