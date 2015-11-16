using UnityEngine;
using System.Collections;

public class HealthyUnit : UnitBase
{
    public override void setStats()
    {
        base.setStats();
        this.health = 20 + (10 * level);
        this.speed = .65f + (.1f * level);
        this.power = 7 + (2.5f * level);
        this.red = 7 + (1 * level);
        this.upgradeCost = 4 + (1 * level);
        this.startHP = this.health;
    }
}