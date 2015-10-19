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
        moveUnits(p2Units, -1);
	}

    public void spawnUnit(GameObject unit) {
        Player p = unit.GetComponent<UnitBase>().controller;
        if(p.isServer) {
            p1Units.Add(unit);
        } else {
            p2Units.Add(unit);
        }
    }

    private void moveUnits(List<GameObject> units, int direction) {
        for (int i = 0; i < units.Count; i++) {
            units[i].transform.Translate(new Vector3(direction * units[i].GetComponent<UnitBase>().speed * Time.deltaTime, 0, 0));
        }
    }
}
