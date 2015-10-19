using UnityEngine;
using System.Collections.Generic;

public class MatchInfo {
    private List<GameObject> matches;
    
    public IEnumerable<GameObject> matchedTiles {
        get {
            return matches;//.Distinct();
        }
    }

    public void add(GameObject go)
    {
        if (!matches.Contains(go))
            matches.Add(go);
    }

    public void addRange(IEnumerable<GameObject> gos) {
        foreach (var item in gos) {
            add(item);
        }
    }

    public MatchInfo()
    {
        matches = new List<GameObject>();
    }
}
