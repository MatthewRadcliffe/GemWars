using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MatchInput : NetworkBehaviour {

    public MatchBoard board;
    public LayerMask tileLayer;
    private GameObject currentlySelected;
    private string myBoard;
    //private bool boardCreated;

    public override void OnStartLocalPlayer()
    {
        print("im Starting up");
        board = null;
        //boardCreated = false;

        if (isServer)
            myBoard = "PlayerOneBoard";
        else
            myBoard = "PlayerTwoBoard";

        board = this.gameObject.GetComponent<MatchBoard>();
        if(myBoard.Equals("PlayerOneBoard"))
        {
            board.startingX = -8;
        }

        board.CmdSetupBoard();
    }

    void Update() {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if(currentlySelected == null) {
                selectTile();
            } else {
                swap();
            }
        }
	}

    private RaycastHit2D tileHit() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50.0f, tileLayer);
        return hit;
    }

    private void selectTile() {
        RaycastHit2D hit = tileHit();
        if(hit) {
            TileInfo info = hit.collider.gameObject.GetComponent<TileInfo>();
            if (info.boardID == board.ID) {
                currentlySelected = hit.collider.gameObject;
            }
        }
    }

    private void swap() {
        RaycastHit2D hit = tileHit();
        if(hit) {
            if(checkAdjacent(hit.collider.gameObject)) {
                board.CmdSwap(currentlySelected, hit.collider.gameObject);
                List<GameObject> temp = (List<GameObject>)board.CheckMatches(currentlySelected);
                remove(temp);
                temp = (List<GameObject>)board.CheckMatches(hit.collider.gameObject);
                remove(temp);
            }
        }
        currentlySelected = null;
    }

    private void remove(IEnumerable<GameObject> list) {
        foreach (GameObject obj in list) {
            board.CmdRemove(obj);
        }
    }

    private bool checkAdjacent(GameObject other) {
        int xDelta = (int)Mathf.Abs(currentlySelected.transform.position.x - other.transform.position.x);
        int yDelta = (int)Mathf.Abs(currentlySelected.transform.position.y - other.transform.position.y);

        bool result = false;
        if (xDelta + yDelta == 1) {
            result = true;
        }
        return result;
    }
}
