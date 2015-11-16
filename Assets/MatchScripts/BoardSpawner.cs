using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BoardSpawner : NetworkBehaviour {
    private GameObject board;
    private Vector2 offScreen = new Vector2(1000, 0);
    private bool gameStarted;
    private bool boardInitialized;

    public GameObject[] prefabs;
    public GameObject p1Indicator, p2Indicator;

    void Awake () {
        gameStarted = false;
        boardInitialized = false;
    }

    private bool checkRematch() {
        int counter = 0;
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder) {
            if(go.GetComponent<Player>().getRematchStatus()) {
                counter++;
            }
        }
        return counter == 2;
    }

    private void resetPlayers() {
        GameObject[] playerHolder = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerHolder) {
            go.GetComponent<Player>().Cmd_resetPlayer();
        }
    }
	
    private void populateBoard() {
        if (isServer) {
            if (board == null) {
                board = (GameObject)Instantiate(Resources.Load("Board"), new Vector3(0, 0, 0), Quaternion.identity);
                board.name = "Board";
                BoardManager manager = board.AddComponent<BoardManager>();
                manager.InitializeReferences(prefabs, p1Indicator, p2Indicator);
                NetworkServer.Spawn(board);
            } else {
                board.GetComponent<BoardManager>().resetBoard();
            }
        }
    }

	void Update () {
        if (!gameStarted) {
            if (GameObject.FindGameObjectsWithTag("Player").Length == 2) {
                gameStarted = true;
                GameObject.Find("WaitingPanel").transform.position = offScreen;
                GameObject.Find("InstructionPanel").transform.position = offScreen;
            }
            return;
        }

        if (!boardInitialized)  {
            boardInitialized = true;
            populateBoard();
        }
        
        if(checkRematch()) {
            GameObject.Find("WaitingPanel").transform.position = offScreen;
            populateBoard();
            resetPlayers();
        }
    }
}
