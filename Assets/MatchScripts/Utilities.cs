using UnityEngine;

public class Utilities : MonoBehaviour {
    public static bool AreNeighbors(TileInfo t1, TileInfo t2) {
        return (t1.column == t2.column || t1.row == t2.row)
            && Mathf.Abs(t1.column - t2.column) <= 1
            && Mathf.Abs(t1.row - t2.row) <= 1;
    }
}
