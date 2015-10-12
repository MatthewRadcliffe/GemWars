using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchInput : MonoBehaviour {

    private MatchBoard board;
    public LayerMask tileLayer;
    private GameObject currentlySelected;
    
	void Awake() {
        board = this.GetComponent<MatchBoard>();
	}
	
	void Update() {
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
            currentlySelected = hit.collider.gameObject;
        }
    }

    private void swap() {
        RaycastHit2D hit = tileHit();
        if(hit) {
            if(checkAdjacent(hit.collider.gameObject)) {
                board.swap(currentlySelected, hit.collider.gameObject);
                List<GameObject> temp = (List<GameObject>)board.checkMatches(currentlySelected);
                remove(temp);
                temp = (List<GameObject>)board.checkMatches(hit.collider.gameObject);
                remove(temp);
            }
        }
        currentlySelected = null;
    }

    private void remove(IEnumerable<GameObject> list) {
        foreach (GameObject obj in list) {
            board.remove(obj);
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
