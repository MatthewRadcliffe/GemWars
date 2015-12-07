using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

public class UnitBase : NetworkBehaviour {
    public Player controller;
    public Player opposingPlayer;
    public int level;
    [SyncVar]
    public float health;
    [SyncVar]
    public float startHP;
    public float power;
    public float speed;

    public int yellow;
    public int red;
    public int green;
    public int blue;
    public int purple;
    public int upgradeCost;
    public bool isColliding;
    public List<GameObject> collidingUnits;

    public void Awake()
    {
        collidingUnits = new List<GameObject>();
        isColliding = false;
        findControllers();
    }

    public void OnCollisionEnter2D(Collision2D coll)
    {
        if (collidingUnits.Count != 0 && (collidingUnits[0].transform.name.Equals("Player1Base") || collidingUnits[0].transform.name.Equals("Player2Base")))
            collidingUnits.Insert(0, coll.gameObject);
        else
            collidingUnits.Add(coll.gameObject);
    }

    public void unitDied(GameObject unit)
    {
        collidingUnits.Remove(unit);
    }

    public void killMe()
    {
        foreach(GameObject go in collidingUnits)
        {
            if (go.GetComponent<UnitBase>() != null)
                go.GetComponent<UnitBase>().unitDied(this.gameObject);
        }

        GameObject deathAnimation = (GameObject)Instantiate(Resources.Load("DeathAnimation"), this.transform.position, Quaternion.identity);
        NetworkServer.Spawn(deathAnimation);
        Destroy(deathAnimation, 1.0f);
        Destroy(this.gameObject);
    }

    public virtual void attack()
    {
        if(collidingUnits.Count != 0)
        {
            if(collidingUnits[0].transform.name.Equals("Player1Base")) {
                opposingPlayer.health -= power * Time.deltaTime;
                health -= 4 * Time.deltaTime;
            }
            else if (collidingUnits[0].transform.name.Equals("Player2Base")) {
                opposingPlayer.health -= power * Time.deltaTime;
                health -= 4 * Time.deltaTime;
            }
            else {
                collidingUnits[0].GetComponent<UnitBase>().health -= power * Time.deltaTime;
            }

        }
    }

    public virtual void Update()
    {
        if (collidingUnits.Count == 0)
            isColliding = false;
        else
            isColliding = true;

        this.transform.FindChild("Canvas").transform.FindChild("HeathBar").transform.FindChild("Image").GetComponent<Image>().fillAmount = health / startHP;

    }

    public virtual void setStats() { }

    public virtual void upgrade()
    {
        level++;
    }

    private void findControllers()
    {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder)
        {
            if ((this.gameObject.transform.position.x < 0 && go.GetComponent<Player>().playerNum == 1) ||
                (this.gameObject.transform.position.x > 0 && go.GetComponent<Player>().playerNum != 1)) {
                controller = go.GetComponent<Player>();
            }
            else {
                opposingPlayer = go.GetComponent<Player>();
            }
        }
    }
}
