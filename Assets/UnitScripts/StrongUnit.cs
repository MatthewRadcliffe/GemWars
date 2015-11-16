using UnityEngine;
using System.Collections;

public class StrongUnit : UnitBase
{
    public override void setStats()
    {
        base.setStats();
        this.health = 18 + (3 * level);
        this.speed = .5f + (.075f * level);
        this.power = 10 + (3.5f * level);
        this.blue = 8 + (1 * level);
        this.upgradeCost = 6 + (1 * level);
        this.startHP = this.health;
    }
}