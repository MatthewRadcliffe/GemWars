using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollapseTiles : MonoBehaviour {
    private List<GameObject> newTiles { get; set; }
    
    public int maxDistance { get; set; }

    public IEnumerable<GameObject> changedTiles {
        get {
            return newTiles; //RETURN DISTINCT
        }
    }

    public CollapseTiles() {
        newTiles = new List<GameObject>();
    }

    public void addTile(GameObject tile) {
        if(!newTiles.Contains(tile)) {
            newTiles.Add(tile);
        }
    }
}
