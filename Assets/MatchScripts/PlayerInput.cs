using UnityEngine;
using UnityEngine.Networking;

public class PlayerInput : NetworkBehaviour {
    private GameState stateP1, stateP2;
    private GameObject selectIndicatorP1, selectIndicatorP2;

    private GameObject hitP1 = null, hitP2 = null;
    private RaycastHit2D target;

    private Vector2 OffScreen = new Vector2(1000, 0);

    private float pause = 1.3f;
    private float cooldownP1, cooldownP2;

    private int lastMoved;

	void Start () {
        stateP1 = GameState.Idle;
        stateP2 = GameState.Idle;
    }

    public void initIndicators(GameObject p1, GameObject p2) {
        selectIndicatorP1 = GameObject.Instantiate(p1) as GameObject;
        selectIndicatorP1.transform.position = OffScreen;
        NetworkServer.Spawn(selectIndicatorP1);

        selectIndicatorP2 = GameObject.Instantiate(p2) as GameObject;
        selectIndicatorP2.transform.position = OffScreen;
        NetworkServer.Spawn(selectIndicatorP2);
    }

    private void SelectorUpdate() {
        if(stateP1 == GameState.SelectedTarget) {
            selectIndicatorP1.transform.position = hitP1.transform.position;
        } else {
            selectIndicatorP1.transform.position = OffScreen;
        }

        if (stateP2 == GameState.SelectedTarget) {
            selectIndicatorP2.transform.position = hitP2.transform.position;
        } else {
            selectIndicatorP2.transform.position = OffScreen;
        }
    }

    public GameObject getInitial() {
        return (lastMoved == 1) ? hitP1 : hitP2;
    }

    public RaycastHit2D getTarget() {
        return target;
    }

    public int currentPlayer() {
        return lastMoved;
    }

    public void reset() {
        hitP1 = hitP2 = null;
        stateP1 = GameState.Idle;
        stateP2 = GameState.Idle;
    }
    
    private void UpdateP1() {
        if (stateP1 == GameState.Idle && cooldownP1 <= 0) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null) {
                stateP1 = GameState.SelectedTarget;
                hitP1 = hit.collider.gameObject;
            }
        }
        else if (stateP1 == GameState.SelectedTarget) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hitP1 != hit.collider.gameObject) {
                if (!Utilities.AreNeighbors(hitP1.GetComponent<TileInfo>(), hit.collider.gameObject.GetComponent<TileInfo>())) {
                    stateP1 = GameState.Idle;
                }
                else {
                    stateP1 = GameState.Animating;
                    lastMoved = 1;
                    target = hit;
                    cooldownP1 = pause;
                }
            }
        }
    }

    private void UpdateP2() {
        if (stateP2 == GameState.Idle && cooldownP2 <= 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                stateP2 = GameState.SelectedTarget;
                hitP2 = hit.collider.gameObject;
            }
        }
        else if (stateP2 == GameState.SelectedTarget)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hitP2 != hit.collider.gameObject)
            {
                if (!Utilities.AreNeighbors(hitP2.GetComponent<TileInfo>(), hit.collider.gameObject.GetComponent<TileInfo>()))
                {
                    stateP2 = GameState.Idle;
                }
                else
                {
                    stateP2 = GameState.Animating;
                    lastMoved = 2;
                    target = hit;
                    cooldownP2 = pause;
                }
            }
        }
    }

    public void onLeftClick(int playerNum) {
        if(playerNum == 1) {
            UpdateP1();
        } else {
            UpdateP2();
        }
    }

    public void onRightClick(int playerNum) {
        if(playerNum == 1) {
            if(stateP1 == GameState.SelectedTarget) {
                stateP1 = GameState.Idle;
            }
        } else {
            if(stateP2 == GameState.SelectedTarget) {
                stateP2 = GameState.Idle;
            }
        }
    }

    public GameState commonUpdate() {
        GameState result = GameState.Paused;
        
        if(stateP1 != GameState.Paused || stateP2 != GameState.Paused) {
            if(stateP1 == GameState.Animating || stateP2 == GameState.Animating) {
                stateP1 = stateP2 = GameState.Paused;
                result = GameState.Animating;
            }
        }

        SelectorUpdate();
        
        if(cooldownP1 > 0) { cooldownP1 -= Time.deltaTime; }
        else { cooldownP1 = 0; }
        if(cooldownP2 > 0) { cooldownP2 -= Time.deltaTime; }
        else { cooldownP2 = 0; }
        
        return result;
    }
}