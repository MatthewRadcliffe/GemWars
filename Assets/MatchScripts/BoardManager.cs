using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
    private MatchBoard board;
    private PlayerInput input;
    private ResourceGiver giver;
    private AudioSource audio;
    private AudioClip matchSound;

    public readonly Vector2 BottomRight = new Vector2(-3.00f, -3.90f);
    public readonly Vector2 TileSize = new Vector2(1.0f, 1.0f);
    
    private Vector2[] SpawnPositions;
    private GameObject[] TilePrefabs;
    private int combo;
    private bool muted;
    
    public void InitializeReferences(GameObject [] Prefabs, GameObject p1Indicator, GameObject p2Indicator) {
        board = this.gameObject.AddComponent<MatchBoard>();
        input = this.gameObject.AddComponent<PlayerInput>();
        input.initIndicators(p1Indicator, p2Indicator);
        giver = this.gameObject.AddComponent<ResourceGiver>();
        audio = this.gameObject.GetComponent<AudioSource>();
        matchSound = audio.clip;
        muted = false;

        TilePrefabs = Prefabs;
        InitializeTypes();
        InitializeSpawn();
    }
    
    void Update() {
        GameState state = input.commonUpdate();

        muted = GameObject.Find("GameManager").GetComponent<ButtonManager>().muted;

        if(state == GameState.Animating) {
            FixSortingLayer(input.getInitial(), input.getTarget().collider.gameObject);
            StartCoroutine(FindMatchesAndCollapse(input.getInitial(), input.getTarget(), input.currentPlayer()));
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

    public void resetBoard() {
        InitializeSpawn();
    }

    private void InstantiateAndPlace(int row, int column, GameObject newTile) {
        GameObject go = Instantiate(newTile,BottomRight + new Vector2(column * TileSize.x, row * TileSize.y), Quaternion.identity) as GameObject;
        go.GetComponent<TileInfo>().assign(newTile.GetComponent<TileInfo>().type, row, column);
        NetworkServer.Spawn(go);
        go.SetActive(true);
        board[row, column] = go;
    }

    private void SetupSpawnPositions() {
        //create the spawn positions for the new shapes (will pop from the 'ceiling')
        for (int column = 0; column < Constants.columns; column++) {
            SpawnPositions[column] = BottomRight + new Vector2(column * TileSize.x, Constants.rows * TileSize.y);
        }
    }

    private IEnumerator FindMatchesAndCollapse(GameObject t1, RaycastHit2D target, int playerNum) {
        var t2 = target.collider.gameObject;
        board.swap(t1, t2);

        iTween.MoveTo(t1, t2.transform.position, Constants.MoveAnimationDuration);
        iTween.MoveTo(t2, t1.transform.position, Constants.MoveAnimationDuration);
        yield return new WaitForSeconds(Constants.MoveAnimationDuration);

        var matchesInfoT1 = board.checkMatches(t1);
        var matchesInfoT2 = board.checkMatches(t2);
        var totalMatches = matchesInfoT1.matchedTiles.Union(matchesInfoT2.matchedTiles).Distinct();

        //if user's swap didn't create at least a 3-match, undo their swap
        if (totalMatches.Count() < Constants.minMatches) {
            board.undoSwap();
            iTween.MoveTo(t1, t2.transform.position, Constants.MoveAnimationDuration);
            iTween.MoveTo(t2, t1.transform.position, Constants.MoveAnimationDuration);
            yield return new WaitForSeconds(Constants.MoveAnimationDuration);
        }

        combo = 0;
        while (totalMatches.Count() >= Constants.minMatches) {
            combo++;
            audio.pitch = .7f + (.1f * combo);
            if(!muted) {
                audio.PlayOneShot(matchSound, .7f);
            }

            int resourceAmount = Constants.minMatches;
            if(totalMatches.Count() > Constants.minMatches) {
                resourceAmount += (totalMatches.Count() - 3) * 2;
            }
            
            ResourceType typeToGive = totalMatches.ElementAt(0).GetComponent<TileInfo>().type;

            foreach (var item in totalMatches)
            {
                GameObject ps = (GameObject)Instantiate(Resources.Load("gemCollect"), item.transform.position, Quaternion.identity);
                print(item.name);
                switch (item.name)
                {
                    case "redTile(Clone)":
                        ps.GetComponent<ParticleSystemRenderer>().material = (Material)Resources.Load("redMat");
                        break;
                    case "greenTile(Clone)":
                        ps.GetComponent<ParticleSystemRenderer>().material = (Material)Resources.Load("greenMat");
                        break;
                    case "blueTile(Clone)":
                        ps.GetComponent<ParticleSystemRenderer>().material = (Material)Resources.Load("blueMat");
                        break;
                    case "yellowTile(Clone)":
                        ps.GetComponent<ParticleSystemRenderer>().material = (Material)Resources.Load("yellowMat");
                        break;
                    case "purpleTile(Clone)":
                        ps.GetComponent<ParticleSystemRenderer>().material = (Material)Resources.Load("purpleMat");
                        break;
                }
            }

            foreach (var item in totalMatches) {
                animateResource(playerNum, item);
            }

            yield return new WaitForSeconds(Constants.MoveAnimationDuration);

            foreach (var item in totalMatches) {
                board.remove(item);
            }

            giver.giveResource(playerNum, typeToGive, resourceAmount);
            
            var columns = totalMatches.Select(go => go.GetComponent<TileInfo>().column).Distinct(); //get the columns that we had a collapse
            var collapsedTileInfo = board.Collapse(columns); //collapse the ones gone
            var newTileInfo = CreateNewTileInSpecificColumns(columns); //create new ones

            int maxDistance = Mathf.Max(collapsedTileInfo.maxDistance, newTileInfo.maxDistance);

            MoveAndAnimate(newTileInfo.changedTiles, maxDistance);
            MoveAndAnimate(collapsedTileInfo.changedTiles, maxDistance);
            
            //search if there are matches with the new/collapsed items
            totalMatches = board.checkMatches(collapsedTileInfo.changedTiles).Union(board.checkMatches(newTileInfo.changedTiles)).Distinct();
            yield return new WaitForSeconds(Constants.ActionDelay);
        }
        input.reset();
    }

    private void animateResource(int playerNum, GameObject item) {
        Vector3 targetPos = new Vector3(4.2f, 4.1f, 0.0f);
        if(playerNum != 0) {
            targetPos.x *= -1;
        }
        iTween.MoveTo(item, targetPos, Constants.MoveAnimationDuration);
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

                if (Constants.rows - item.row > newTileInfo.maxDistance) {
                    newTileInfo.maxDistance = Constants.rows - item.row;
                }

                board[item.row, item.column] = newTile;
                NetworkServer.Spawn(newTile);
                newTileInfo.addTile(newTile);
            }
        }
        return newTileInfo;
    }

    private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance) {
        foreach (var item in movedGameObjects) {
            Vector3 goal = BottomRight;
            goal += new Vector3(item.GetComponent<TileInfo>().column * TileSize.x, item.GetComponent<TileInfo>().row * TileSize.y, 0);
            iTween.MoveTo(item, goal, Constants.MoveAnimationDuration);
        }
    }

    private GameObject randomTile() {
        int rand = Random.Range(0, 100);// TilePrefabs.Length);

        int index = -1;
        if (rand >= 85) { //15% chance of purple
            index = 4;
        } else if (rand >= 60) { //25% chance of yellow
            index = 3;
        } else if (rand >= 40) { //20% chance of green
            index = 2;
        } else if (rand >= 20) { //20% chance of blue
            index = 1;
        } else { //20% chance of red
            index = 0;
        }
        
        return TilePrefabs[index];
    }

    private void WipeBoard() {
        for (int row = 0; row < Constants.rows; row++) {
            for (int column = 0; column < Constants.columns; column++) {
                Destroy(board[row, column]);
            }
        }
    }
}
