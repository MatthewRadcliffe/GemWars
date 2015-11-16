using UnityEngine;
using System.Collections;

public class BomberUnit : UnitBase {

    private float timer = 0;
    private bool blownUp = false;

    // Use this for initialization
    public override void setStats()
    {
        base.setStats();
        this.health = 8 + (4 * level);
        this.speed = .6f + (.1f * level);
        this.power = 40 + (30f * level);
        this.purple = 6 + (1 * level);
        this.upgradeCost = 6 + (2 * level);
        this.startHP = this.health;
    }

    public override void attack()
    {
        timer += Time.deltaTime;

        if (collidingUnits.Count != 0 && timer >= 1.0f && !blownUp)
        {
            if (collidingUnits[0].transform.name.Equals("Player1Base"))
            {
                opposingPlayer.health -= power;
            }
            else if (collidingUnits[0].transform.name.Equals("Player2Base"))
            {
                opposingPlayer.health -= power;
            }

            foreach(GameObject go in collidingUnits)
            {
                if(go.GetComponent<UnitBase>() != null)
                {
                    go.GetComponent<UnitBase>().health -= power;
                }
            }

            health = 0;
            blownUp = true;
        }
    }

    public override void upgrade()
    {
        base.upgrade();
        if (!isColliding)
            timer = 0;
    }

}
