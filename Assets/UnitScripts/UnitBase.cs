using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class UnitBase : NetworkBehaviour {
    public Player controller;
    public int level;
    public float health;
    public float power;
    public float speed;

    public int yellow;
    public int red;
    public int green;
    public int blue;
    public int purple;
    public int upgradeCost;

    public void spawnMinion(Player p)
    {
        
    }

    public virtual void upgrade()
    {
        level++;
    }
}
