using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CollapseTiles {
    private List<GameObject> newTiles { get; set; }
    
    public int maxDistance { get; set; }

    public IEnumerable<GameObject> changedTiles {
        get {
            return newTiles.Distinct();
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
