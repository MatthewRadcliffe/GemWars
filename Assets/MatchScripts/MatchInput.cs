//using UnityEngine;
//using System.Collections.Generic;

//public class MatchInput : MonoBehaviour {

//    public MatchBoard board;
//    public LayerMask tileLayer;
//    private GameObject currentlySelected;
//    private string myBoard;
//    //private bool boardCreated;
    
//    public void Awake()
//    {
//        board = null;
//        board = this.gameObject.GetComponent<MatchBoard>();

//        board.makeBoard();
//    }

//    void Update() {
//        if (Input.GetKeyDown(KeyCode.Mouse0)) {
//            if(currentlySelected == null) {
//                selectTile();
//            } else {
//                swap();
//            }
//        }
//	}

//    private RaycastHit2D tileHit() {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50.0f, tileLayer);
//        return hit;
//    }

//    private void selectTile() {
//        RaycastHit2D hit = tileHit();
//        if(hit) {
//            TileInfo info = hit.collider.gameObject.GetComponent<TileInfo>();
//            currentlySelected = hit.collider.gameObject;
//        }
//    }

//    private void swap() {
//        RaycastHit2D hit = tileHit();
//        if(hit) {
//            if(checkAdjacent(hit.collider.gameObject)) {
//                board.CmdSwap(currentlySelected, hit.collider.gameObject);
//                List<GameObject> temp = (List<GameObject>)board.checkMatches(currentlySelected);
//                remove(temp);
//                temp = (List<GameObject>)board.checkMatches(hit.collider.gameObject);
//                remove(temp);
//            }
//        }
//        currentlySelected = null;
//    }

//    private void remove(IEnumerable<GameObject> list) {
//        foreach (GameObject obj in list) {
//            board.CmdRemove(obj);
//        }
//    }

//    private bool checkAdjacent(GameObject other) {
//        int xDelta = (int)Mathf.Abs(currentlySelected.transform.position.x - other.transform.position.x);
//        int yDelta = (int)Mathf.Abs(currentlySelected.transform.position.y - other.transform.position.y);

//        bool result = false;
//        if (xDelta + yDelta == 1) {
//            result = true;
//        }
//        return result;
//    }
//}
