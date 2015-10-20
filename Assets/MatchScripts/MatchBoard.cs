using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MatchBoard : MonoBehaviour {
    private GameObject[,] grid = new GameObject[Constants.rows, Constants.columns];
    
    private GameObject backupG1;
    private GameObject backupG2;

    public GameObject this[int row, int column] {
        get {
            try {
                return grid[row, column];
            } catch (Exception e) {
                throw;
            }
        }
        set {
            grid[row, column] = value;
        }
    }
 
    public void swap(GameObject g1, GameObject g2) {
        backupG1 = g1;
        backupG2 = g2;

        TileInfo g1Info = g1.GetComponent<TileInfo>();
        TileInfo g2Info = g2.GetComponent<TileInfo>();
        
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

        if (info.column != Constants.rows - 1) {
            for (int c = info.column + 1; c < Constants.columns; c++) {
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

        if (info.row != Constants.rows - 1) {
            for (int r = info.row + 1; r < Constants.columns; r++) {
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

    public MatchInfo checkMatches(GameObject obj) {
        MatchInfo info = new MatchInfo();
        List<GameObject> horizontal = (List<GameObject>)checkMatchesHorizontal(obj);
        info.addRange(horizontal);
        List<GameObject> vertical = (List<GameObject>)checkMatchesVertical(obj);
        info.addRange(vertical);
        return info;
    }

    public IEnumerable<GameObject> checkMatches(IEnumerable<GameObject> objects) {
        List<GameObject> matches = new List<GameObject>();
        foreach(GameObject obj in objects) {
            matches.AddRange(checkMatches(obj).matchedTiles);
        }
        return matches;
    }
    
    public void remove(GameObject obj) {
        TileInfo info = obj.GetComponent<TileInfo>();
        grid[info.row, info.column] = null;
        Destroy(obj);
    }

    public CollapseTiles Collapse(IEnumerable<int> columns) {
        CollapseTiles collapseInfo = new CollapseTiles();
        
        ///search in every column
        foreach (var column in columns) {
            //begin from bottom row
            for (int row = 0; row < Constants.rows - 1; row++) {
                //if you find a null item
                if (grid[row, column] == null) {
                    //start searching for the first non-null
                    for (int row2 = row + 1; row2 < Constants.rows; row2++) {
                        //if you find one, bring it down (i.e. replace it with the null you found)
                        if (grid[row2, column] != null) {
                            grid[row, column] = grid[row2, column];
                            grid[row2, column] = null;

                            //calculate the biggest distance
                            if (row2 - row > collapseInfo.maxDistance)
                                collapseInfo.maxDistance = row2 - row;

                            //assign new row and column (name does not change)
                            grid[row, column].GetComponent<TileInfo>().row = row;
                            grid[row, column].GetComponent<TileInfo>().column = column;

                            collapseInfo.addTile(grid[row, column]);
                            break;
                        }
                    }
                }
            }
        }

        return collapseInfo;
    }

    public IEnumerable<TileInfo> GetAllEmptyInColumn(int column) {
        List<TileInfo> emptyTiles = new List<TileInfo>();
        for(int r = 0; r < Constants.rows; r++) {
            if(grid[r, column] == null) {
                emptyTiles.Add(new TileInfo() { row = r, column = column });
            }
        }
        return emptyTiles;
    }
}