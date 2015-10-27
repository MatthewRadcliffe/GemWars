using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BoardSpawner : NetworkBehaviour {
    private bool gameStarted;
    private bool boardInitialized;

    public GameObject[] prefabs;

    public GameObject p1Indicator, p2Indicator;

    void Awake () {
        gameStarted = false;
        boardInitialized = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameStarted) {
            if (GameObject.FindGameObjectsWithTag("Player").Length == 2) {
                gameStarted = true;
                GameObject.Find("WaitingPanel").transform.position = new Vector2(1000, 0);
            }
            return;
        }

        if (!boardInitialized)
        {
            boardInitialized = true;
            if (isServer) {
                GameObject board = (GameObject)Instantiate(Resources.Load("Board"), new Vector3(0, 0, 0), Quaternion.identity);
                board.name = "Board";
                BoardManager manager = board.AddComponent<BoardManager>();
                manager.InitializeReferences(prefabs, p1Indicator, p2Indicator);
                NetworkServer.Spawn(board);
            }
        }
    }
}
