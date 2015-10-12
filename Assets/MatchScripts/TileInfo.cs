using UnityEngine;
using System.Collections;

public class TileInfo : MonoBehaviour {
    public int column { get; set; }
    public int row { get; set; }
    public int type { get; set; }
    public int boardID { get; set; }
    
    public void assign(int boardID, int type, int row, int col) {
        column = col;
        this.row = row;
        this.type = type;
        this.boardID = boardID;
    }

    public static void swap(TileInfo one, TileInfo two)
    {
        int temp = one.row;
        one.row = two.row;
        two.row = temp;
        temp = one.column;
        one.column = two.column;
        two.column = temp;

        Vector3 tempPos = one.transform.position;
        one.transform.position = two.transform.position;
        two.transform.position = tempPos;
    }

    public bool areSame(TileInfo other) {
        if(other == null || !(other is TileInfo) ) {
            throw new System.ArgumentException("uncomparable other tile");
        }
        return type == other.type && boardID == other.boardID;
    }
}