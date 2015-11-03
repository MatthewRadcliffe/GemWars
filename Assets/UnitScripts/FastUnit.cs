using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class FastUnit : UnitBase { 

    public override void setStats()
    {
        base.setStats();
        this.health = 12 + (3 * level);
        this.speed = 1.1f + (.2f * level);
        this.power = 4 + (1.75f * level);
        this.green = 3 + (1 * level);
        this.upgradeCost = 2 + (1 * level);
    }
}
