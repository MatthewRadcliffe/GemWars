using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class FastUnit : UnitBase { 

    public override void setStats()
    {
        base.setStats();
        this.health = 10 + (2 * level);
        this.speed = 1.1f + (.15f * level);
        this.power = 4 + (1.75f * level);
        this.green = 7 + (1 * level);
        this.upgradeCost = 3 + (1 * level);
    }
}
