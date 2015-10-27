using UnityEngine.Networking;

public class TileInfo : NetworkBehaviour {
    public int column { get; set; }
    public int row { get; set; }
    public ResourceType type { get; set; }
    
    public void assign(ResourceType type, int row, int col) {
        column = col;
        this.row = row;
        this.type = type;
    }

    public static void swap(TileInfo one, TileInfo two) {
        int temp = one.row;
        one.row = two.row;
        two.row = temp;
        temp = one.column;
        one.column = two.column;
        two.column = temp;
    }

    public bool areSame(TileInfo other) {
        if(other == null || !(other is TileInfo) ) {
            throw new System.ArgumentException("uncomparable other tile");
        }
        return type == other.type;
    }
}