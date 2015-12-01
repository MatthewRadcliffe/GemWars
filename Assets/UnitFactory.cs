using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class UnitFactory : NetworkBehaviour {
    private List<GameObject> p1Units;
    private List<GameObject> p2Units;
    
	void Awake () {
        p1Units = new List<GameObject>();
        p2Units = new List<GameObject>();
	}

	void Update () {
        moveUnits(p1Units, 1);
        moveUnits(p2Units, 1);
	}

    public void spawnUnit(GameObject unit, Player p) {
        //Player p = unit.GetComponent<UnitBase>().controller;
        if(unit.transform.position.x < 0) {
            unit.gameObject.layer = 9;
            p1Units.Add(unit);
        } else {
            unit.transform.localRotation = Quaternion.Euler(0, 180, 0);
            p2Units.Add(unit);
            unit.gameObject.layer = 10;
        }
    }

    private void moveUnits(List<GameObject> units, int direction) {
        for (int i = units.Count - 1; i >= 0; i--) {
            if(units[i].GetComponent<UnitBase>().health <= 0) {
                GameObject temp = units[i];
                units.Remove(temp);
                temp.GetComponent<UnitBase>().killMe();
            } else {
                if (!units[i].GetComponent<UnitBase>().isColliding) {
                    units[i].transform.Translate(new Vector3(direction * units[i].GetComponent<UnitBase>().speed * Time.deltaTime, 0, 0));
                }
                else {
                    units[i].GetComponent<UnitBase>().attack();
                }
            } 
        }

        //for (int i = 0; i < units.Count; i++) {
        //    if (!units[i].GetComponent<UnitBase>().isColliding) {
        //        units[i].transform.Translate(new Vector3(direction * units[i].GetComponent<UnitBase>().speed * Time.deltaTime, 0, 0));
        //    } else {
        //        units[i].GetComponent<UnitBase>().attack();
        //    }
        //}
    }
}
