using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchBoard : MonoBehaviour {
    public int GridWidth, GridHeight;
    public int startingX, startingY;
    public int ID;
    public GameObject[,] grid;

    public GameObject[] TilePrefabs;

    private GameObject backupG1;
    private GameObject backupG2;

    void Awake() {
        CreateGrid();
    }

    private void CreateGrid() {
        grid = new GameObject[GridWidth, GridHeight];

        for (int x = 0; x < GridWidth; x++) {
            for (int y = 0; y < GridHeight; y++) {
                grid[x, y] = makeRandom(x, y);
            }
        }
    }

    private GameObject makeRandom(int x, int y) {
        int randomTile = Random.Range(0, TilePrefabs.Length);
        GameObject newTile = (GameObject)Instantiate(TilePrefabs[randomTile], new Vector2(startingX + x, startingY + y), Quaternion.identity);
        newTile.GetComponent<TileInfo>().assign(ID, randomTile, x, y);
        return newTile;
    }

    public void swap(GameObject g1, GameObject g2) {
        backupG1 = g1;
        backupG2 = g2;

        TileInfo g1Info = g1.GetComponent<TileInfo>();
        TileInfo g2Info = g2.GetComponent<TileInfo>();

        //print("Swapping (" + g1Info.row + ", " + g1Info.column + ") of board " + g1Info.boardID + " with (" + g2Info.row + ", " + g2Info.column + ") of board " + g2Info.boardID);

        int g1R = g1Info.row;
        int g1C = g1Info.column;
        int g2R = g2Info.row;
        int g2C = g2Info.column;

        GameObject temp = grid[g1R, g1C];
        grid[g1R, g1C] = grid[g2R, g2C];
        grid[g2R, g2C] = temp;

        TileInfo.swap(g1Info, g2Info);
    }

    public void undoSwap() {
        if(backupG1 == null || backupG2 == null) {
            throw new System.Exception("Null backup");
        }

        swap(backupG1, backupG2);
    }

    public IEnumerable<GameObject> checkMatchesHorizontal(GameObject obj) {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(obj);
        TileInfo info = obj.GetComponent<TileInfo>();

        if(info.column != 0) {
            for(int c = info.column -1; c >= 0; c--) {
                if(grid[info.row, c].GetComponent<TileInfo>().areSame(info)) {
                    matches.Add(grid[info.row, c]);
                } else {
                    break;
                }
            }
        }

        if (info.column != GridWidth - 1) {
            for (int c = info.column + 1; c < GridWidth; c++) {
                if (grid[info.row, c].GetComponent<TileInfo>().areSame(info)) {
                    matches.Add(grid[info.row, c]);
                }
                else {
                    break;
                }
            }
        }

        if(matches.Count < 3) {
            matches.Clear();
        }

        return matches;
    }

    public IEnumerable<GameObject> checkMatchesVertical(GameObject obj) {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(obj);
        TileInfo info = obj.GetComponent<TileInfo>();

        if (info.row != 0) {
            for (int r = info.row - 1; r >= 0; r--) {
                if(grid[r, info.column] != null && grid[r, info.column].GetComponent<TileInfo>().areSame(info)) {
                    matches.Add(grid[r, info.column]);
                }
                else {
                    break;
                }
            }
        }

        if (info.row != GridHeight - 1) {
            for (int r = info.row + 1; r < GridHeight; r++) {
                if(grid[r, info.column] != null && grid[r, info.column].GetComponent<TileInfo>().areSame(info)) {
                    matches.Add(grid[r, info.column]);
                }
                else {
                    break;
                }
            }
        }

        if (matches.Count < 3) {
            matches.Clear();
        }

        return matches;
    }

    public IEnumerable<GameObject> checkMatches(GameObject obj) {
        List<GameObject> horizontal = (List<GameObject>)checkMatchesHorizontal(obj);
        List<GameObject> vertical = (List<GameObject>)checkMatchesVertical(obj);
        horizontal.AddRange(vertical);
        return horizontal;
    }

    public void checkMatches(IEnumerable<GameObject> objects) {
        List<GameObject> matches = new List<GameObject>();
        foreach(GameObject obj in objects) {
            //matches.AddRange(checkMatches(obj))
        }
    }

    public void remove(GameObject obj) {
        TileInfo info = obj.GetComponent<TileInfo>();
        grid[info.row, info.column] = makeRandom(info.row, info.column);
        Destroy(obj);
    }
}