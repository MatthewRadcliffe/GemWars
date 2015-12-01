using UnityEngine;
using System.Collections;

public class FlyingUnit : UnitBase {
    public override void setStats()
    {
        base.setStats();
        this.health = 8 + (4 * level);
        this.speed = .6f + (.1f * level);
        this.power =  3f + (2f * level);
        this.green = 2 + ((1 * level) / 2);
        this.purple = 2 + ((1 * level) / 2);
        this.upgradeCost = 3 + (1 * level);
        this.startHP = this.health;
    }
}
