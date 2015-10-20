using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
    public MatchBoard board;
    private GameObject selectIndicator;
    
    public readonly Vector2 BottomRight = new Vector2(-3.00f, -3.90f);
    public readonly Vector2 TileSize = new Vector2(1.0f, 1.0f);
    
    private GameState state = GameState.Idle;
    private GameObject hitGo = null;
    private Vector2[] SpawnPositions;
    public GameObject[] TilePrefabs;
    
    void Awake () {
        InitializeTypes();
        InitializeSpawn();
        selectIndicator = GameObject.Find("SelectIndicator");
    }

    void Update() {
        if (state == GameState.Idle) {
            selectIndicator.transform.position = new Vector2(-500, 0);
            if (Input.GetMouseButtonDown(0)) {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null) {
                    hitGo = hit.collider.gameObject;
                    state = GameState.SelectedTarget;
                }
            }
        }
        else if (state == GameState.SelectedTarget) {
            if (Input.GetMouseButton(0)) {
                selectIndicator.transform.position = hitGo.transform.position;
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hitGo != hit.collider.gameObject) {
                    if (!Utilities.AreNeighbors(hitGo.GetComponent<TileInfo>(),
                        hit.collider.gameObject.GetComponent<TileInfo>())) {
                        state = GameState.Idle;
                    } else {
                        state = GameState.Animating;
                        //FixSortingLayer(hitGo, hit.collider.gameObject);
                        StartCoroutine(FindMatchesAndCollapse(hit));
                    }
                }
            }
        }
    }

    private void InitializeTypes() {
        for(int i = 0; i < TilePrefabs.Length; i++) {
            TilePrefabs[i].GetComponent<TileInfo>().type = (ResourceType)i;
        }
    }
    
    private void InitializeSpawn() {
        if (board != null) {
            WipeBoard();
        }

        SpawnPositions = new Vector2[Constants.columns];

        for (int row = 0; row < Constants.rows; row++) {
            for (int column = 0; column < Constants.columns; column++) {
                GameObject newTile = randomTile();
                
                //check if two previous horizontal are of the same type
                while (column >= 2 && board[row, column - 1].GetComponent<TileInfo>().areSame(newTile.GetComponent<TileInfo>())
                    && board[row, column - 2].GetComponent<TileInfo>().areSame(newTile.GetComponent<TileInfo>())) {
                    newTile = randomTile();
                }
                
                //check if two previous vertical are of the same type
                while (row >= 2 && board[row - 1, column].GetComponent<TileInfo>().areSame(newTile.GetComponent<TileInfo>())
                    && board[row - 2, column].GetComponent<TileInfo>().areSame(newTile.GetComponent<TileInfo>())) {
                    newTile = randomTile();
                }

                InstantiateAndPlace(row, column, newTile);
            }
        }

        SetupSpawnPositions();
    }

    private void InstantiateAndPlace(int row, int column, GameObject newTile) {
        GameObject go = Instantiate(newTile,
            BottomRight + new Vector2(column * TileSize.x, row * TileSize.y), Quaternion.identity)
            as GameObject;
        
        go.GetComponent<TileInfo>().assign(newTile.GetComponent<TileInfo>().type, row, column);
        go.SetActive(true);
        board[row, column] = go;
    }

    private void SetupSpawnPositions() {
        //create the spawn positions for the new shapes (will pop from the 'ceiling')
        for (int column = 0; column < Constants.columns; column++) {
            SpawnPositions[column] = BottomRight + new Vector2(column * TileSize.x, Constants.rows * TileSize.y);
        }
    }

    private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit2) {
        var hitGo2 = hit2.collider.gameObject;
        board.swap(hitGo, hitGo2);

        yield return new WaitForSeconds(Constants.AnimationDuration);

        var hitGomatchesInfo = board.checkMatches(hitGo);
        var hitGo2matchesInfo = board.checkMatches(hitGo2);

        var totalMatches = hitGomatchesInfo.matchedTiles.Union(hitGo2matchesInfo.matchedTiles).Distinct();

        //if user's swap didn't create at least a 3-match, undo their swap
        if (totalMatches.Count() < Constants.minMatches) {
            board.undoSwap();
            yield return new WaitForSeconds(Constants.AnimationDuration);
        }
        
        while (totalMatches.Count() >= Constants.minMatches) {
            int resourceAmount = Constants.minMatches;
            if(totalMatches.Count() > Constants.minMatches) {
                resourceAmount += (totalMatches.Count() - 3) * 2;
            }

            this.GetComponent<ResourceGiver>().giveResource(hitGo.GetComponent<TileInfo>().type, resourceAmount);

            foreach (var item in totalMatches) {
                board.remove(item);
            }

            //get the columns that we had a collapse
            var columns = totalMatches.Select(go => go.GetComponent<TileInfo>().column).Distinct();

            //the order the 2 methods below get called is important!!!
            //collapse the ones gone
            var collapsedTileInfo = board.Collapse(columns);
            //create new ones
            var newTileInfo = CreateNewTileInSpecificColumns(columns);

            int maxDistance = Mathf.Max(collapsedTileInfo.maxDistance, newTileInfo.maxDistance);

            MoveAndAnimate(newTileInfo.changedTiles, maxDistance);
            MoveAndAnimate(collapsedTileInfo.changedTiles, maxDistance);
            
            //search if there are matches with the new/collapsed items
            totalMatches = board.checkMatches(collapsedTileInfo.changedTiles).Union(board.checkMatches(newTileInfo.changedTiles)).Distinct();
        }
        state = GameState.Idle;
    }

    private void FixSortingLayer(GameObject hitGo, GameObject hitGo2) {
        SpriteRenderer sp1 = hitGo.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitGo2.GetComponent<SpriteRenderer>();
        if (sp1.sortingOrder <= sp2.sortingOrder) {
            sp1.sortingOrder = 1;
            sp2.sortingOrder = 0;
        }
    }

    private CollapseTiles CreateNewTileInSpecificColumns(IEnumerable<int> columnsWithMissingTile) {
        CollapseTiles newTileInfo = new CollapseTiles();
        
        foreach (int column in columnsWithMissingTile) {
            var emptyItems = board.GetAllEmptyInColumn(column);
            foreach (var item in emptyItems) {
                var go = randomTile();
                GameObject newTile = Instantiate(go, SpawnPositions[column], Quaternion.identity) as GameObject;

                newTile.GetComponent<TileInfo>().assign(go.GetComponent<TileInfo>().type, item.row, item.column);

                if (Constants.rows - item.row > newTileInfo.maxDistance)
                    newTileInfo.maxDistance = Constants.rows - item.row;

                board[item.row, item.column] = newTile;
                newTileInfo.addTile(newTile);
            }
        }
        return newTileInfo;
    }

    private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance) {
        foreach (var item in movedGameObjects) {
            item.transform.position = BottomRight + new Vector2(item.GetComponent<TileInfo>().column * TileSize.x, item.GetComponent<TileInfo>().row * TileSize.y);
        }
    }

    private GameObject randomTile() {
        return TilePrefabs[UnityEngine.Random.Range(0, TilePrefabs.Length)];
    }

    private void WipeBoard() {
        for (int row = 0; row < Constants.rows; row++) {
            for (int column = 0; column < Constants.columns; column++) {
                Destroy(board[row, column]);
            }
        }
    }
}
