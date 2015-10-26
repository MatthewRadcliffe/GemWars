using UnityEngine;
using System.Collections;

public class HealthyUnit : UnitBase
{
    public override void setStats()
    {
        base.setStats();
        this.health = 24 + (5 * level);
        this.speed = .65f + (.1f * level);
        this.power = 6 + (2.5f * level);
        this.red = 10 + (1 * level);
        this.upgradeCost = 4 + (1 * level);
    }
}